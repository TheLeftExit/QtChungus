using System.Linq.Expressions;
using System.Reflection;

namespace QtChungus.Internal
{
    public class PropertyBinding<TViewModel, TWidget, TValue> : ISignalHandler<TViewModel>
        where TViewModel : class
        where TWidget : struct, IQObject
    {
        public required QObject WidgetHandle { private get; init; }
        public required string? SignalName { private get; init; }
        public required WidgetGetMethod<TWidget, TValue> WidgetGetMethod { private get; init; }
        public required WidgetSetMethod<TWidget, TValue>? WidgetSetMethod { private get; init; }

        public required string? ViewModelPropertyName { private get; set; }
        public required ViewModelGetMethod<TViewModel, TValue> ViewModelGetMethod { private get; init; }
        public required ViewModelSetMethod<TViewModel, TValue>? ViewModelSetMethod { private get; init; }

        public PropertyBinding()
        {
            CVariant.GetTypeCodeOrThrow<TValue>();
        }

        public void ProcessSignal(QObject sender, ReadOnlySpan<char> name, TViewModel viewModel)
        {
            if (sender != WidgetHandle) return;
            if (!name.SequenceEqual(SignalName)) return;

            Push(viewModel, PushDirection.ToViewModel);
        }

        public void ProcessPropertyChanged(TViewModel sender, string? propertyName)
        {
            if (propertyName is not null && propertyName != ViewModelPropertyName) return;
            Push(sender, PushDirection.ToWidget);
        }

        private enum PushDirection { ToWidget, ToViewModel }
        private void Push(TViewModel viewModel, PushDirection direction)
        {
            if (direction == PushDirection.ToWidget && WidgetSetMethod is null) return;
            if (direction == PushDirection.ToViewModel && ViewModelSetMethod is null) return;

            var widget = new TWidget { Handle = WidgetHandle };

            var widgetValue = WidgetGetMethod(widget);
            var viewModelValue = ViewModelGetMethod(viewModel);
            if (EqualityComparer<TValue>.Default.Equals(widgetValue, viewModelValue)) return;

            switch (direction)
            {
                case PushDirection.ToWidget:
                    WidgetSetMethod!(widget, viewModelValue);
                    break;
                case PushDirection.ToViewModel:
                    ViewModelSetMethod!(viewModel, widgetValue);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}

namespace QtChungus
{
    public static partial class ViewBuilderExtensions
    {
        public static void SetBinding<TViewModel, TWidget, TValue>(
            this IViewBuilder<TViewModel> builder,
            TWidget widget,
            Expression<Func<TWidget, TValue>> widgetPropertySelector,
            Expression<Func<TViewModel, TValue>> viewModelPropertySelector
        )
            where TViewModel : class
            where TWidget : struct, IQWidget
        {
            if (!ExpressionHelper.TryParseProperty(widgetPropertySelector, out var controlProperty))
            {
                throw new NotSupportedException();
            }
            if (!ExpressionHelper.TryParseProperty(viewModelPropertySelector, out var viewModelProperty))
            {
                throw new NotSupportedException();
            }

            var signalInfo = controlProperty.GetCustomAttribute<QtSignalAttribute>();
            if (signalInfo is not null)
            {
                SignalRouter.ConnectSignal(widget.Handle, signalInfo.SignalSignature);
            }

            var handler = new PropertyBinding<TViewModel, TWidget, TValue>
            {
                WidgetHandle = widget.Handle,
                WidgetGetMethod = controlProperty.GetMethod!.CreateDelegate<WidgetGetMethod<TWidget, TValue>>(),
                WidgetSetMethod = controlProperty.SetMethod?.CreateDelegate<WidgetSetMethod<TWidget, TValue>>(),
                SignalName = signalInfo?.SignalName,
                ViewModelGetMethod = viewModelProperty.GetMethod!.CreateDelegate<ViewModelGetMethod<TViewModel, TValue>>(),
                ViewModelSetMethod = viewModelProperty.SetMethod?.CreateDelegate<ViewModelSetMethod<TViewModel, TValue>>(),
                ViewModelPropertyName = viewModelProperty.Name
            };
            builder.AddSignalHandler(handler);
        }
    }
}
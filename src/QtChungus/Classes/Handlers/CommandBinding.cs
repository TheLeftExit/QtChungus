using System.Linq.Expressions;
using System.Reflection;

namespace QtChungus.Internal
{
    public class CommandBinding<TViewModel, TWidget> : ISignalHandler<TViewModel>
        where TViewModel : class
        where TWidget : struct, IQObject
    {
        public required QObject WidgetHandle { private get; init; }
        public required string SignalName { private get; init; }
        public required ViewModelCommand<TViewModel> ViewModelCommandMethod { private get; init; }

        public void ProcessSignal(QObject sender, ReadOnlySpan<char> name, TViewModel viewModel)
        {
            if (sender != WidgetHandle) return;
            if (!name.SequenceEqual(SignalName)) return;

            ViewModelCommandMethod(viewModel);
        }

        public void ProcessPropertyChanged(TViewModel sender, string propertyName) { }
    }
}


namespace QtChungus
{
    public static partial class ViewBuilderExtensions
    {
        public static void SetCommand<TViewModel, TWidget>(
            this IViewBuilder<TViewModel> builder,
            TWidget widget,
            Expression<Func<TWidget, Action>> widgetCommandSelector,
            Expression<Func<TViewModel, Action>> viewModelCommandSelector
        )
            where TViewModel : class
            where TWidget : struct, IQObject
        {
            if (!ExpressionHelper.TryParseMethod(widgetCommandSelector, out var controlCommand))
            {
                throw new NotSupportedException();
            }
            if (controlCommand.GetCustomAttribute<QtSignalAttribute>() is not QtSignalAttribute signalInfo)
            {
                throw new NotSupportedException();
            }
            if (!ExpressionHelper.TryParseMethod(viewModelCommandSelector, out var viewModelCommand))
            {
                throw new NotSupportedException();
            }
            if (viewModelCommand.ReturnType != typeof(void) || viewModelCommand.GetParameters().Length != 0)
            {
                throw new NotSupportedException();
            }

            SignalRouter.ConnectSignal(widget.Handle, signalInfo.SignalSignature);

            builder.AddSignalHandler(new CommandBinding<TViewModel, TWidget>
            {
                SignalName = signalInfo.SignalName,
                ViewModelCommandMethod = viewModelCommand.CreateDelegate<ViewModelCommand<TViewModel>>(),
                WidgetHandle = widget.Handle
            });
        }
    }
}

using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace QtChungus.Internal
{
    public class ListModelBinding<TViewModel, TWidget, TRow> : ISignalHandler<TViewModel>
        where TViewModel : class
        where TWidget : struct, IQObject
        where TRow : class
    {
        public required QObjectStar WidgetHandle { private get; init; }
        public required string? ViewModelPropertyName { private get; set; }
        public required ViewModelGetMethod<TViewModel, IList<TRow>> ViewModelGetMethod { private get; init; }
        public required IQModelColumn<TRow>[] ModelColumns { private get; init; }

        void ISignalHandler<TViewModel>.ProcessSignal(QObjectStar sender, ReadOnlySpan<char> name, TViewModel viewModel)
        {
            return;
        }

        void ISignalHandler<TViewModel>.ProcessPropertyChanged(TViewModel sender, string? propertyName)
        {
            if (propertyName is not null && propertyName != ViewModelPropertyName) return;

            var view = QAbstractItemView.FromHandle(WidgetHandle);

            var oldNativeModel = view.Model;
            if(oldNativeModel.Handle != default && oldNativeModel.Handle.Inherits(nameof(QcAbstractItemModel)))
            {
                ((QcAbstractItemModel_Unmanaged)oldNativeModel).ManagedModel.Dispose();
            }

            var newList = ViewModelGetMethod(sender);
            var newModel = new QListModel<TRow>(newList, ModelColumns);
            view.Model = newModel.UnmanagedModel;
        }
    }

    public class ListModelBindingBuilder<TViewModel, TRow>(
        QObjectStar widgetHandle,
        string? viewModelPropertyName,
        ViewModelGetMethod<TViewModel, IList<TRow>> viewModelGetMethod
    )
        where TViewModel : class
        where TRow : class
    {
        private readonly List<IQModelColumn<TRow>> _columns = new();

        public void AddColumn<TValue>(Expression<Func<TRow, TValue>> accessor, string? caption = null)
        {
            _columns.Add(new QModelColumn<TRow, TValue>(accessor, caption));
        }

        public ListModelBinding<TViewModel, TWidget, TRow> Build<TWidget>() where TWidget : struct, IQObject
        {
            return new ListModelBinding<TViewModel, TWidget, TRow>
            {
                WidgetHandle = widgetHandle,
                ViewModelGetMethod = viewModelGetMethod,
                ViewModelPropertyName = viewModelPropertyName,
                ModelColumns = _columns.ToArray()
            };
        }
    }
}

namespace QtChungus
{
    public static partial class ViewBuilderExtensions
    {
        public static void SetModel<TViewModel, TWidget, TRow>(
            this ISignalManager<TViewModel> builder,
            TWidget widget,
            Expression<Func<TViewModel, IList<TRow>>> viewModelPropertySelector,
            Action<ListModelBindingBuilder<TViewModel, TRow>> configure
        )
            where TViewModel : class
            where TWidget : struct, IQObject
            where TRow : class
        {
            if (!ExpressionHelper.TryParseProperty(viewModelPropertySelector, out var viewModelProperty))
            {
                throw new NotSupportedException();
            }

            var bindingBuilder = new ListModelBindingBuilder<TViewModel, TRow>(
                widget.Handle,
                viewModelProperty.Name,
                viewModelProperty.GetMethod!.CreateDelegate<ViewModelGetMethod<TViewModel, IList<TRow>>>()
            );
            configure(bindingBuilder);
            var binding = bindingBuilder.Build<TWidget>();
            builder.AddSignalHandler(binding);
        }
    }
}



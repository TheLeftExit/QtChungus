using System.Linq.Expressions;
using System.Runtime.InteropServices;

public class ListModelBinding<TViewModel, TWidget, TRow> : ISignalHandler<TViewModel>
    where TViewModel : class
    where TWidget : struct, IQWidget
    where TRow : class
{
    public required QObject WidgetHandle { private get; init; }
    public required string? ViewModelPropertyName { private get; set; }
    public required ViewModelGetMethod<TViewModel, IList<TRow>> ViewModelGetMethod { private get; init; }
    public required IQModelColumn<TRow>[] ModelColumns { private get; init; }

    public void ProcessSignal(QObject sender, ReadOnlySpan<char> name, TViewModel viewModel)
    {
        return;
    }

    public void ProcessPropertyChanged(TViewModel sender, string? propertyName)
    {
        if (propertyName is not null && propertyName != ViewModelPropertyName) return;

        var oldNativeModel = Qt.qabstractitemview_model(WidgetHandle);
        var oldModel = oldNativeModel.Handle != 0 ? QAbstractItemModel.FromHandle(oldNativeModel.Handle) : null;

        var newList = ViewModelGetMethod(sender);
        var newModel = new QListModel<TRow>(newList, ModelColumns);
        Qt.qabstractitemview_setmodel(WidgetHandle, newModel.Handle);

        oldModel?.Dispose();
    }
}

public static partial class ViewBuilderExtensions
{
    public static void SetModel<TViewModel, TWidget, TRow>(
        this IViewBuilder<TViewModel> builder,
        QWidgetHandle<TWidget> widget,
        Expression<Func<TViewModel, IList<TRow>>> viewModelPropertySelector,
        IQModelColumn<TRow>[] columns
    )
        where TViewModel : class
        where TWidget : struct, IQWidget
        where TRow: class
    {
        if (!ExpressionHelper.TryParseProperty(viewModelPropertySelector, out var viewModelProperty))
        {
            throw new NotSupportedException();
        }

        var handler = new ListModelBinding<TViewModel, TWidget, TRow>
        {
            WidgetHandle = widget.Handle,
            ViewModelGetMethod = viewModelProperty.GetMethod!.CreateDelegate<ViewModelGetMethod<TViewModel, IList<TRow>>>(),
            ViewModelPropertyName = viewModelProperty.Name,
            ModelColumns = columns
        };
        builder.AddSignalHandler(handler);
    }
}
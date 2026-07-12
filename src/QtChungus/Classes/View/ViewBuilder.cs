using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;

public record struct QWidgetHandle<TWidget>(QObject Handle) where TWidget : IQWidget, new()
{
    public static QWidgetHandle<TWidget> CreateWidget(Action<TWidget>? configureWidget, QObject parent)
    {
        var widgetHandle = QObject.CreateWidgetCore(TWidget.ClassName, parent);
        var widget = new TWidget { Handle = widgetHandle };
        widget.SetDefaults();
        configureWidget?.Invoke(widget);
        return new(widget.Handle);
    }
}

public interface IViewBuilder<TViewModel>
{
    void AddSignalHandler(ISignalHandler<TViewModel> signalHandler);
}

public class QMainWindowBuilder<TViewModel> : IViewBuilder<TViewModel>
    where TViewModel : class
{
    private readonly QObject _handle;
    private readonly List<ISignalHandler<TViewModel>> _signalHandlers;

    public QMainWindowBuilder(Action<QMainWindow>? configureWindow = null)
    {
        _handle = QObject.CreateWidgetCore("QMainWindow", default);
        _signalHandlers = new();
        configureWindow?.Invoke(new(_handle));
    }

    public QWidgetHandle<TWidget> AddWidget<TWidget>(Action<TWidget>? configureWidget = null) where TWidget : IQWidget, new()
    {
        // TODO: validate that we have no layout
        return QWidgetHandle<TWidget>.CreateWidget(configureWidget, _handle);
    }

    public TLayout UseLayout<TLayout>() where TLayout : IQLayout, new()
    {
        // TODO: validate that we have 0 controls and no layout
        var centralWidgetHandle = QObject.CreateWidgetCore("QWidget", default);
        Qt.qmainwindow_setcentralwidget(_handle, centralWidgetHandle);
        var layoutHandle = QObject.CreateWidgetCore(TLayout.ClassName, centralWidgetHandle);
        return new TLayout { Handle = layoutHandle };
    }

    public void AddSignalHandler(ISignalHandler<TViewModel> signalHandler) => _signalHandlers.Add(signalHandler);

    public View<TViewModel> Build()
    {
        return new(_handle, _signalHandlers.ToArray());
    }
}

// QHBoxLayout/QVBoxLayout:
// new(Orientation)
// AddWidget(weight, alignment), AddLayout(weight), AddEmptySpace(weight)

// QGridLayout:
// SetRowProperties(index, weight, minLength)
// SetColumnProperties(index, weight, minLength)
// AddWidget(row, column, rowSpan, columnSpan, alignment), AddLayout(row, column, rowSpan, columnSpan, alignment)

// QFormLayout:
// AddWidget(caption)
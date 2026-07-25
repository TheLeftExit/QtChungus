using System.Drawing;

namespace QtChungus;

public readonly record struct QMainWindow(QObject Handle) : IQWindow
{
    public static string ClassName => "QMainWindow";

    public Size Size
    {
        get => Handle.GetProperty<Size>("size");
        set => Handle.SetProperty("size", value);
    }

    public string WindowTitle
    {
        get => Handle.GetProperty<string>("windowTitle");
        set => Handle.SetProperty("windowTitle", value);
    }

    // =====

    public TWidget AddWidget<TWidget>(Action<TWidget>? configureWidget) where TWidget : struct, IQWidget
    {
        return QObject.Create<TWidget>(configureWidget, Handle);
    }

    public TLayout UseLayout<TLayout>() where TLayout : struct, IQLayout
    {
        var centralWidget = QObject.Create<QWidget>();
        Handle.SetProperty<QObject>("centralWidget", centralWidget.Handle);
        var layout = QObject.Create<TLayout>(null, centralWidget.Handle);
        return layout;
    }

    public void Show()
    {
        Qt.qmainwindow_show(Handle);
    }
}
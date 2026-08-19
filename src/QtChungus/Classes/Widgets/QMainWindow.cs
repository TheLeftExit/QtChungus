using System.Drawing;

namespace QtChungus;

public readonly record struct QMainWindow(QObject Handle) : IQContainerWidget
{
    static string IQObject.ClassName => "QMainWindow";
    void IQObject.SetDefaults()
    {
        CentralWidget = QObject.Create<QWidget>();
    }
    QObject IQContainerWidget.ContainerWidget => CentralWidget.Handle;

    public TLayout UseLayout<TLayout>() where TLayout : struct, IQLayout
    {
        return QWidgetHelper.UseLayout<QMainWindow, TLayout>(this);
    }

    private QWidget CentralWidget
    {
        get => new(Handle.GetProperty<QObject>("centralWidget"));
        set => Handle.SetProperty<QObject>("centralWidget", value.Handle);
    }

    public QLayoutSizeConstraint SizeConstraint
    {
        get => Handle.GetProperty<QObject>("layout").GetProperty<QLayoutSizeConstraint>("sizeConstraint");
        set => Handle.GetProperty<QObject>("layout").SetProperty<QLayoutSizeConstraint>("sizeConstraint", value);
    }

    public Size Size
    {
        get => Handle.GetProperty<Size>("size");
        set => Handle.SetProperty("size", value);
    }

    public string? WindowTitle
    {
        get => Handle.GetProperty<string?>("windowTitle");
        set => Handle.SetProperty("windowTitle", value);
    }
}

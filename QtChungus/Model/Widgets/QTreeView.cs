using System.Drawing;

public readonly record struct QTreeView(QObject Handle) : IWidget
{
    public static string ClassName => "QTreeView";

    void IWidget.SetDefaults()
    {
        Handle.SetProperty("rootIsDecorated", false);
    }

    public Rectangle Bounds
    {
        get => Handle.GetProperty<Rectangle>("geometry");
        set => Handle.SetProperty("geometry", value);
    }
}
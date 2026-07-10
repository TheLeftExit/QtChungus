using System.Drawing;

public readonly record struct QTreeView(QObject Handle) : IQWidget
{
    public static string ClassName => "QTreeView";

    void IQWidget.SetDefaults()
    {
        Handle.SetProperty("rootIsDecorated", false);
    }

    public Rectangle Bounds
    {
        get => Handle.GetProperty<Rectangle>("geometry");
        set => Handle.SetProperty("geometry", value);
    }
}
using System.Drawing;

namespace QtChungus;

public readonly record struct QTreeView(QObject Handle) : IQWidget
{
    public static string ClassName => "QTreeView";

    void IQObject.SetDefaults()
    {
        Handle.SetProperty("rootIsDecorated", false);
    }

    public Rectangle Bounds
    {
        get => Handle.GetProperty<Rectangle>("geometry");
        set => Handle.SetProperty("geometry", value);
    }
}
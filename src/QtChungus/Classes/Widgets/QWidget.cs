using System.Drawing;

namespace QtChungus.Internal;

// Not a base type; just a plain QWidget for when we need one (e.g. QMainWindow's central widget)
public readonly record struct QWidget(QObject Handle) : IQWidget
{
    public static string ClassName => "QWidget";

    public Rectangle Bounds
    {
        get => Handle.GetProperty<Rectangle>("geometry");
        set => Handle.SetProperty("geometry", value);
    }
}
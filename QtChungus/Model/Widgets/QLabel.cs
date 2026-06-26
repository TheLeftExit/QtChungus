using System.Drawing;

public readonly record struct QLabel(QObject Handle) : IWidget
{
    public static string ClassName => "QLabel";

    public Rectangle Bounds
    {
        get => Handle.GetProperty<Rectangle>("geometry");
        set => Handle.SetProperty("geometry", value);
    }

    public string Text
    {
        get => Handle.GetProperty<string>("text");
        set => Handle.SetProperty("text", value);
    }
}

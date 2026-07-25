using System.Drawing;

namespace QtChungus;

public readonly record struct QLineEdit(QObject Handle) : IQWidget
{
    public static string ClassName => "QLineEdit";

    public Rectangle Bounds
    {
        get => Handle.GetProperty<Rectangle>("geometry");
        set => Handle.SetProperty("geometry", value);
    }

    [QtSignal("textChanged", "textChanged(QString)")]
    public string Text
    {
        get => Handle.GetProperty<string>("text");
        set => Handle.SetProperty("text", value);
    }

    public string PlaceholderText
    {
        get => Handle.GetProperty<string>("placeholderText");
        set => Handle.SetProperty("placeholderText", value);
    }
}

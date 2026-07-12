using System.Drawing;

public readonly record struct QMainWindow(QObject Handle) : IQWidget
{
    public static string ClassName => throw new NotSupportedException();

    public Size Size
    {
        get => Handle.GetProperty<Size>("size");
        set => Handle.SetProperty("size", value);
    }

    public string Text
    {
        get => Handle.GetProperty<string>("text");
        set => Handle.SetProperty("text", value);
    }
}
using System.Drawing;

public readonly record struct QMainWindow(QObject Handle) : IQWidget
{
    public static string ClassName => throw new NotSupportedException();

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
}
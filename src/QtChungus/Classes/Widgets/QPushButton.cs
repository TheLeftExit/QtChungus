using System.Drawing;

public readonly record struct QPushButton(QObject Handle) : IQWidget
{
    public static string ClassName => "QPushButton";

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

    [QtSignal("clicked", "clicked()")]
    public void OnClicked() => throw new NotSupportedException();
}

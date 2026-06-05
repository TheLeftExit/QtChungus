using System.Drawing;
using System.Reflection.Metadata;

public readonly struct QPushButton : IWidget<QPushButton>
{
    public readonly QObject Handle { get; init; }
    public static QPushButton New(QObject parent) => new() { Handle = QObject.CreateWidget("QPushButton", parent) };

    public Rectangle Bounds
    {
        get => Handle.GetRectangle("geometry");
        set => Handle.SetRectangle("geometry", value);
    }

    public string Text
    {
        get => Handle.GetString("text");
        set => Handle.SetString("text", value);
    }

    [QtSignal("clicked", "clicked()")]
    public void OnClicked() => throw new NotSupportedException();
}

public readonly struct QLabel : IWidget<QLabel>
{
    public readonly QObject Handle { get; init; }
    public static QLabel New(QObject parent) => new() { Handle = QObject.CreateWidget("QLabel", parent) };

    public Rectangle Bounds
    {
        get => Handle.GetRectangle("geometry");
        set => Handle.SetRectangle("geometry", value);
    }

    public string Text
    {
        get => Handle.GetString("text");
        set => Handle.SetString("text", value);
    }
}
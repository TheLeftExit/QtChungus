using System.Drawing;

public readonly struct PushButton : IWidget<PushButton>
{
    public readonly QObject Handle { get; init; }
    public static PushButton New(QObject parent) => new() { Handle = Qt.qpushbutton_new(parent) };

    public unsafe string Text
    {
        get => new(Qt.qabstractbutton_text(Handle));
        set => Qt.qabstractbutton_settext(Handle, value);
    }

    public Rectangle Bounds
    {
        get => Qt.qwidget_geometry(Handle);
        set => Qt.qwidget_setgeometry(Handle, value);
    }

    [QtSignal("clicked", "clicked()")]
    public void OnClicked() => throw new NotSupportedException();
}
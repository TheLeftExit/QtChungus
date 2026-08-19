using System.Drawing;

namespace QtChungus;

public readonly record struct QPushButton(QObject Handle) : IQWidget
{
    static string IQObject.ClassName => "QPushButton";

    public string? Text
    {
        get => Handle.GetProperty<string?>("text");
        set => Handle.SetProperty("text", value);
    }

    [QtSignal("clicked", "clicked()")]
    public void OnClicked() => throw new NotSupportedException();
}

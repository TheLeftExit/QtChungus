using System.Drawing;

namespace QtChungus;

public readonly record struct QLineEdit(QObject Handle) : IQWidget
{
    static string IQObject.ClassName => "QLineEdit";

    [QtSignal("textChanged", "textChanged(QString)")]
    public string? Text
    {
        get => Handle.GetProperty<string?>("text");
        set => Handle.SetProperty("text", value);
    }

    public string? PlaceholderText
    {
        get => Handle.GetProperty<string?>("placeholderText");
        set => Handle.SetProperty("placeholderText", value);
    }
}

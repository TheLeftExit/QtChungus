using System.Drawing;

namespace QtChungus;

public readonly record struct QLabel(QObject Handle) : IQWidget
{
    static string IQObject.ClassName => "QLabel";

    void IQObject.SetDefaults()
    {
        TextFormat = QtTextFormat.MarkdownText;
    }

    public string? Text
    {
        get => Handle.GetProperty<string?>("text");
        set => Handle.SetProperty("text", value);
    }

    public QtTextFormat TextFormat
    {
        get => Handle.GetProperty<QtTextFormat>("textFormat");
        set => Handle.SetProperty("textFormat", value);
    }
}

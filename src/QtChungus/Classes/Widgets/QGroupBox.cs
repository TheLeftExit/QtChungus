namespace QtChungus;

public readonly record struct QGroupBox(QObject Handle) : IQContainerWidget
{
    static string IQObject.ClassName => "QGroupBox";

    public TLayout UseLayout<TLayout>() where TLayout : struct, IQLayout
    {
        return QWidgetHelper.UseLayout<QGroupBox, TLayout>(this);
    }

    public string? Title
    {
        get => Handle.GetProperty<string?>("title");
        set => Handle.SetProperty("title", value);
    }
}
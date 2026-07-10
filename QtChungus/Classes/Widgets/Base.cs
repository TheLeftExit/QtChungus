public interface IQWidget
{
    QObject Handle { get; init; }
    abstract static string ClassName { get; }
    void SetDefaults() { }
}

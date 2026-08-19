namespace QtChungus;

public readonly record struct QFrame(QObject Handle) : IQWidget
{
    static string IQObject.ClassName => "QFrame";
}
namespace QtChungus.Internal;

public interface IQObject
{
    QObjectStar Handle { get; init; }
}

public interface IQWidget : IQObject;
public interface IQLayout : IQObject;
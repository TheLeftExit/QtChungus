namespace QtChungus.Internal
{
    // Class markers to restrict, for instance, calling AddWidget on layouts or other non-widget IQObject-s.
    public interface IQWidget : IQObject;
    public interface IQLayout : IQObject;

    public interface IQWindow : IQObject
    {
        void Show();
    }
}


namespace QtChungus
{
    public static class WindowExtensions
    {
        extension<T>(T window) where T : struct, IQWindow
        {
            public static T Create(Action<T>? configure = null)
            {
                return QObject.Create<T>(configure);
            }
        }
    }
}

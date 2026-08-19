namespace QtChungus.Internal
{
    public interface IQLayout : IQObject
    {
        QLayoutSizeConstraint SizeConstraint { get; set; }
    }
    public interface IQLayout<TLayoutItemArgs> : IQLayout where TLayoutItemArgs : struct
    {
        TWidget AddWidget<TWidget>(TLayoutItemArgs args) where TWidget : struct, IQWidget;
        TLayout AddLayout<TLayout>(TLayoutItemArgs args) where TLayout : struct, IQLayout;
    }

    public interface IQWidget : IQObject;

    public interface IQContainerWidget : IQWidget
    {
        QObject ContainerWidget => Handle;
        TLayout UseLayout<TLayout>() where TLayout : struct, IQLayout;
    }
}


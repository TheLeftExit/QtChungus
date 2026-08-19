namespace QtChungus;

public readonly record struct QBoxLayoutItemArgs(int Weight = 0, QtAlignment Alignment = default);

public readonly record struct QBoxLayout(QObject Handle) : IQLayout<QBoxLayoutItemArgs>
{
    static string IQObject.ClassName => "QBoxLayout";
    public QLayoutSizeConstraint SizeConstraint
    {
        get => Handle.GetProperty<QLayoutSizeConstraint>("sizeConstraint");
        set => Handle.SetProperty("sizeConstraint", value);
    }

    public QBoxLayoutDirection Direction
    {
        get => Handle.GetProperty<QBoxLayoutDirection>("direction");
        set => Handle.SetProperty("direction", value);
    }

    public TWidget AddWidget<TWidget>(QBoxLayoutItemArgs args = default)
        where TWidget : struct, IQWidget
    {
        var widget = QObject.Create<TWidget>();
        Qt.qboxlayout_addwidget(Handle, widget.Handle, args.Weight, args.Alignment);
        return widget;
    }

    public TLayout AddLayout<TLayout>(QBoxLayoutItemArgs args = default) where TLayout : struct, IQLayout
    {
        var layout = QObject.Create<TLayout>();
        Qt.qboxlayout_addlayout(Handle, layout.Handle, args.Weight /* no alignment :c */);
        return layout;
    }
}
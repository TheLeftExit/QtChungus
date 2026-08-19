namespace QtChungus;

public readonly record struct QFormLayoutItemArgs(string Caption);

public readonly record struct QFormLayout(QObject Handle) : IQLayout<QFormLayoutItemArgs>
{
    static string IQObject.ClassName => "QFormLayout";
    public QLayoutSizeConstraint SizeConstraint
    {
        get => Handle.GetProperty<QLayoutSizeConstraint>("sizeConstraint");
        set => Handle.SetProperty("sizeConstraint", value);
    }

    public TWidget AddWidget<TWidget>(QFormLayoutItemArgs args)
        where TWidget : struct, IQWidget
    {
        var widget = QObject.Create<TWidget>();
        Qt.qformlayout_addwidget(Handle, args.Caption, widget.Handle);
        return widget;
    }

    public TLayout AddLayout<TLayout>(QFormLayoutItemArgs args)
        where TLayout : struct, IQLayout
    {
        var layout = QObject.Create<TLayout>();
        Qt.qformlayout_addlayout(Handle, args.Caption, layout.Handle);
        return layout;
    }
}
namespace QtChungus;

public readonly record struct QGridLayoutItemArgs(int Row, int Column, int RowSpan = 1, int ColumnSpan = 1, QtAlignment Alignment = default);

public readonly record struct QGridLayout(QObject Handle) : IQLayout<QGridLayoutItemArgs>
{
    static string IQObject.ClassName => "QGridLayout";
    public QLayoutSizeConstraint SizeConstraint
    {
        get => Handle.GetProperty<QLayoutSizeConstraint>("sizeConstraint");
        set => Handle.SetProperty("sizeConstraint", value);
    }

    public QGridLayoutRows Rows => new(Handle);
    public QGridLayoutColumns Columns => new(Handle);

    public TWidget AddWidget<TWidget>(QGridLayoutItemArgs args)
        where TWidget : struct, IQWidget
    {
        var widget = QObject.Create<TWidget>();
        Qt.qgridlayout_addwidget(Handle, widget.Handle, args.Row, args.Column, args.RowSpan, args.ColumnSpan, args.Alignment);
        return widget;
    }

    public TLayout AddLayout<TLayout>(QGridLayoutItemArgs args)
        where TLayout : struct, IQLayout
    {
        var layout = QObject.Create<TLayout>();
        Qt.qgridlayout_addlayout(Handle, layout.Handle, args.Row, args.Column, args.RowSpan, args.ColumnSpan, args.Alignment);
        return layout;
    }
}

public readonly record struct QGridLayoutRows(QObject Layout)
{
    public QGridLayoutRow this[int index] => new(Layout, index);
}

public readonly record struct QGridLayoutRow(QObject Layout, int Row)
{
    public void SetMinimumHeight(int height) => Qt.qgridlayout_setrowminimumheight(Layout, Row, height);
    public void SetStretch(int stretch) => Qt.qgridlayout_setrowstretch(Layout, Row, stretch);
}

public readonly record struct QGridLayoutColumns(QObject Layout)
{
    public QGridLayoutColumn this[int index] => new(Layout, index);
}

public readonly record struct QGridLayoutColumn(QObject Layout, int Column)
{
    public void SetMinimumWidth(int width) => Qt.qgridlayout_setcolumnminimumwidth(Layout, Column, width);
    public void SetStretch(int stretch) => Qt.qgridlayout_setcolumnstretch(Layout, Column, stretch);
}
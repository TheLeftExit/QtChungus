public interface IQLayout
{
    QObject Handle { get; init; }
    abstract static string ClassName { get; }
}

public readonly record struct QHBoxLayout(QObject Handle) : IQLayout
{
    public static string ClassName => "QHBoxLayout";

    public QWidgetHandle<TWidget> AddWidget<TWidget>(Action<TWidget>? configureWidget = null, int weight = 0, QtAlignment alignment = default)
        where TWidget : IQWidget, new()
    {
        var widget = QWidgetHandle<TWidget>.CreateWidget(configureWidget, default);
        Qt.qboxlayout_addwidget(Handle, widget.Handle, weight, alignment);
        return widget;
    }

    public TLayout AddLayout<TLayout>(int weight = 0) where TLayout : IQLayout, new()
    {
        var layoutHandle = QObject.CreateWidgetCore(TLayout.ClassName, default);
        Qt.qboxlayout_addlayout(Handle, layoutHandle, weight);
        return new TLayout { Handle = layoutHandle };
    }
}

public readonly record struct QVBoxLayout(QObject Handle) : IQLayout
{
    public static string ClassName => "QVBoxLayout";

    public QWidgetHandle<TWidget> AddWidget<TWidget>(Action<TWidget>? configureWidget = null, int weight = 0, QtAlignment alignment = default)
        where TWidget : IQWidget, new()
    {
        var widget = QWidgetHandle<TWidget>.CreateWidget(configureWidget, default);
        Qt.qboxlayout_addwidget(Handle, widget.Handle, weight, alignment);
        return widget;
    }

    public TLayout AddLayout<TLayout>(int weight = 0) where TLayout : IQLayout, new()
    {
        var layoutHandle = QObject.CreateWidgetCore(TLayout.ClassName, default);
        Qt.qboxlayout_addlayout(Handle, layoutHandle, weight);
        return new TLayout { Handle = layoutHandle };
    }
}

public readonly record struct QGridLayout(QObject Handle) : IQLayout
{
    public static string ClassName => "QGridLayout";

    public QWidgetHandle<TWidget> AddWidget<TWidget>(Action<TWidget>? configureWidget, int row, int column, int rowSpan = 1, int columnSpan = 1, QtAlignment alignment = default)
        where TWidget : IQWidget, new()
    {
        var widget = QWidgetHandle<TWidget>.CreateWidget(configureWidget, default);
        Qt.qgridlayout_addwidget(Handle, widget.Handle, row, column, rowSpan, columnSpan, alignment);
        return widget;
    }

    public QWidgetHandle<TWidget> AddWidget<TWidget>(int row, int column, int rowSpan = 1, int columnSpan = 1, QtAlignment alignment = default)
        where TWidget : IQWidget, new()
    {
        return AddWidget<TWidget>(null, row, column, rowSpan, columnSpan, alignment);
    }

    public TLayout AddLayout<TLayout>(int row, int column, int rowSpan = 1, int columnSpan = 1, QtAlignment alignment = default) where TLayout : IQLayout, new()
    {
        var layoutHandle = QObject.CreateWidgetCore(TLayout.ClassName, default);
        Qt.qgridlayout_addlayout(Handle, layoutHandle, row, column, rowSpan, columnSpan, alignment);
        return new TLayout { Handle = layoutHandle };
    }
}

public readonly record struct QFormLayout(QObject Handle) : IQLayout
{
    public static string ClassName => "QFormLayout";

    public QWidgetHandle<TWidget> AddWidget<TWidget>(Action<TWidget>? configureWidget, ReadOnlySpan<char> caption)
        where TWidget : IQWidget, new()
    {
        var widget = QWidgetHandle<TWidget>.CreateWidget(configureWidget, default);
        Qt.qformlayout_addwidget(Handle, caption, widget.Handle);
        return widget;
    }

    public QWidgetHandle<TWidget> AddWidget<TWidget>(ReadOnlySpan<char> caption)
        where TWidget : IQWidget, new()
    {
        return AddWidget<TWidget>(null, caption);
    }

    public TLayout AddLayout<TLayout>(ReadOnlySpan<char> caption) where TLayout : IQLayout, new()
    {
        var layoutHandle = QObject.CreateWidgetCore(TLayout.ClassName, default);
        Qt.qformlayout_addlayout(Handle, caption, layoutHandle);
        return new TLayout { Handle = layoutHandle };
    }
}
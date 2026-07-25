namespace QtChungus;

public readonly record struct QHBoxLayout(QObject Handle) : IQLayout
{
    public static string ClassName => "QHBoxLayout";

    public TWidget AddWidget<TWidget>(Action<TWidget>? configureWidget = null, int weight = 0, QtAlignment alignment = default)
        where TWidget : IQWidget, new()
    {
        var widget = QObject.Create(configureWidget, default);
        Qt.qboxlayout_addwidget(Handle, widget.Handle, weight, alignment);
        return widget;
    }

    public TLayout AddLayout<TLayout>(int weight = 0) where TLayout : IQLayout, new()
    {
        var layout = QObject.Create<TLayout>();
        Qt.qboxlayout_addlayout(Handle, layout.Handle, weight);
        return layout;
    }
}

public readonly record struct QVBoxLayout(QObject Handle) : IQLayout
{
    public static string ClassName => "QVBoxLayout";

    public TWidget AddWidget<TWidget>(Action<TWidget>? configureWidget = null, int weight = 0, QtAlignment alignment = default)
        where TWidget : IQWidget, new()
    {
        var widget = QObject.Create(configureWidget, default);
        Qt.qboxlayout_addwidget(Handle, widget.Handle, weight, alignment);
        return widget;
    }

    public TLayout AddLayout<TLayout>(int weight = 0) where TLayout : IQLayout, new()
    {
        var layout = QObject.Create<TLayout>();
        Qt.qboxlayout_addlayout(Handle, layout.Handle, weight);
        return layout;
    }
}

public readonly record struct QGridLayout(QObject Handle) : IQLayout
{
    public static string ClassName => "QGridLayout";

    public TWidget AddWidget<TWidget>(Action<TWidget>? configureWidget, int row, int column, int rowSpan = 1, int columnSpan = 1, QtAlignment alignment = default)
        where TWidget : IQWidget, new()
    {
        var widget = QObject.Create(configureWidget);
        Qt.qgridlayout_addwidget(Handle, widget.Handle, row, column, rowSpan, columnSpan, alignment);
        return widget;
    }

    public TWidget AddWidget<TWidget>(int row, int column, int rowSpan = 1, int columnSpan = 1, QtAlignment alignment = default)
        where TWidget : IQWidget, new()
    {
        return AddWidget<TWidget>(null, row, column, rowSpan, columnSpan, alignment);
    }

    public TLayout AddLayout<TLayout>(int row, int column, int rowSpan = 1, int columnSpan = 1, QtAlignment alignment = default) where TLayout : IQLayout, new()
    {
        var layout = QObject.Create<TLayout>();
        Qt.qgridlayout_addlayout(Handle, layout.Handle, row, column, rowSpan, columnSpan, alignment);
        return layout;
    }
}

public readonly record struct QFormLayout(QObject Handle) : IQLayout
{
    public static string ClassName => "QFormLayout";

    public TWidget AddWidget<TWidget>(Action<TWidget>? configureWidget, ReadOnlySpan<char> caption)
        where TWidget : IQWidget, new()
    {
        var widget = QObject.Create(configureWidget);
        Qt.qformlayout_addwidget(Handle, caption, widget.Handle);
        return widget;
    }

    public TWidget AddWidget<TWidget>(ReadOnlySpan<char> caption)
        where TWidget : IQWidget, new()
    {
        return AddWidget<TWidget>(null, caption);
    }

    public TLayout AddLayout<TLayout>(ReadOnlySpan<char> caption) where TLayout : IQLayout, new()
    {
        var layout = QObject.Create<TLayout>();
        Qt.qformlayout_addlayout(Handle, caption, layout.Handle);
        return layout;
    }
}
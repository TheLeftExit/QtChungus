using System.Reflection.Metadata;

namespace QtChungus.Internal;

public interface IQObject // All QObject descendants
{
    QObjectStar Handle { get; init; }
}
public interface IQChildWidget : IQObject; // Widget usable as a child widget
public interface IQContainerWidget : IQObject // Widget usable as a parent widget
{
    QLayout Layout { get; set; }
}
public interface IQWindow : IQObject, IDisposable // Widget usable as a top-level window
{
    void Show();
    bool Visible { get; }
}
public interface IQLayout : IQObject; // Non-abstract layout

public static class QObjectExtensions
{
    extension<T>(T target) where T : struct, IQObject
    {
        public static T FromHandle(QObjectStar handle) => default(T) with { Handle = handle };
        internal QObjectStar ValidateHandle(QObjectStar handle, string className)
        {
            if (target.Handle != default) throw new InvalidOperationException("Attempted to overwrite a managed QObject's handle.");
            if (!handle.Inherits(className)) throw new InvalidOperationException("Attempted to assign a handle to a mismatched managed QObject type.");
            return handle;
        }
    }

    internal static QLayout AsQLayout<TLayout>(this TLayout layout)
        where TLayout : struct, IQLayout
    {
        return QLayout.FromHandle(layout.Handle);
    }
    internal static QWidget AsQWidget<TWidget>(this TWidget widget)
        where TWidget : struct, IQChildWidget
    {
        return QWidget.FromHandle(widget.Handle);
    }

    private static TLayout UseLayout<TWidget, TLayout>(TWidget widget)
        where TWidget : struct, IQContainerWidget
        where TLayout : struct, IQLayout
    {
        var layout = new TLayout();
        widget.Layout = layout.AsQLayout();
        return layout;
    }

    public static QGridLayout UseGridLayout<T>(this T widget) where T : struct, IQContainerWidget => UseLayout<T, QGridLayout>(widget);
    public static TLayout AddLayout<TLayout>(this QGridLayout target, int row, int column, int rowSpan = 1, int columnSpan = 1, QtAlignment alignment = default)
        where TLayout : struct, IQLayout
    {
        var layout = new TLayout();
        target.AddLayout(layout.AsQLayout(), row, column, rowSpan, columnSpan, alignment);
        return layout;
    }
    public static TWidget AddWidget<TWidget>(this QGridLayout target, int row, int column, int rowSpan = 1, int columnSpan = 1, QtAlignment alignment = default)
        where TWidget : struct, IQChildWidget
    {
        var widget = new TWidget();
        target.AddWidget(widget.AsQWidget(), row, column, rowSpan, columnSpan, alignment);
        return widget;
    }

    public static QFormLayout UseFormLayout<T>(this T widget) where T : struct, IQContainerWidget => UseLayout<T, QFormLayout>(widget);
    public static TLayout AddLayout<TLayout>(this QFormLayout target, string caption)
        where TLayout : struct, IQLayout
    {
        var layout = new TLayout();
        target.AddLayout(layout.AsQLayout(), caption);
        return layout;
    }

    public static TWidget AddWidget<TWidget>(this QFormLayout target, string caption)
        where TWidget : struct, IQChildWidget
    {
        var widget = new TWidget();
        target.AddWidget(widget.AsQWidget(), caption);
        return widget;
    }


    public static QVBoxLayout UseVBoxLayout<T>(this T widget) where T : struct, IQContainerWidget => UseLayout<T, QVBoxLayout>(widget);
    public static TLayout AddLayout<TLayout>(this QVBoxLayout target, int stretch = 0)
        where TLayout : struct, IQLayout
    {
        var layout = new TLayout();
        target.AddLayout(layout.AsQLayout(), stretch);
        return layout;
    }
    public static TWidget AddWidget<TWidget>(this QVBoxLayout target, int stretch = 0, QtAlignment alignment = default)
        where TWidget : struct, IQChildWidget
    {
        var widget = new TWidget();
        target.AddWidget(widget.AsQWidget(), stretch, alignment);
        return widget;
    }

    public static QHBoxLayout UseHBoxLayout<T>(this T widget) where T : struct, IQContainerWidget => UseLayout<T, QHBoxLayout>(widget);
    public static TLayout AddLayout<TLayout>(this QHBoxLayout target, int stretch = 0)
        where TLayout : struct, IQLayout
    {
        var layout = new TLayout();
        target.AddLayout(layout.AsQLayout(), stretch);
        return layout;
    }
    public static TWidget AddWidget<TWidget>(this QHBoxLayout target, int stretch = 0, QtAlignment alignment = default)
        where TWidget : struct, IQChildWidget
    {
        var widget = new TWidget();
        target.AddWidget(widget.AsQWidget(), stretch, alignment);
        return widget;
    }
}

internal static class QObjectHelpers
{
    public static void ThrowAbstractClassException()
    {
        throw new InvalidOperationException($"Attempted to create an instance of an abstract class.");
    }

    public static void ThrowSignalOnlyException()
    {
        throw new InvalidOperationException($"Attempted to invoke a signal marker method.");
    }
}
using System.Drawing;

namespace QtChungus;

public static class QWidgetHelper
{
    public static Size GetSizeHint<T>(Action<T> configure) where T : struct, IQWidget
    {
        var widget = QObject.Create<T>();
        configure(widget);
        var result = widget.Handle.GetProperty<Size>("sizeHint");
        Qt.qobject_delete(widget.Handle);
        return result;
    }

    internal static TLayout UseLayout<TWidget, TLayout>(this TWidget widget)
        where TWidget : struct, IQContainerWidget
        where TLayout : struct, IQLayout
    {
        var layout = QObject.Create<TLayout>();
        widget.ContainerWidget.SetProperty<QObject>("layout", layout.Handle);
        return layout;
    }
}

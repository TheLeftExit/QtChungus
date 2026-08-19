using System.Drawing;

namespace QtChungus.Internal;

public readonly partial record struct QWidget(QObject Handle) : IQContainerWidget
{
    static string IQObject.ClassName => "QWidget";

    public TLayout UseLayout<TLayout>() where TLayout : struct, IQLayout
    {
        return QWidgetHelper.UseLayout<QWidget, TLayout>(this);
    }
}
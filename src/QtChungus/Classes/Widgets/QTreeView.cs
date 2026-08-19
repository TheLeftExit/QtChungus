using System.Drawing;

namespace QtChungus;

public readonly record struct QTreeView(QObject Handle) : IQWidget
{
    static string IQObject.ClassName => "QTreeView";
    void IQObject.SetDefaults()
    {
        Handle.SetProperty("rootIsDecorated", false);
    }
}
#nullable disable

public static partial class QTypes
{
    public static QObjectType QObject = new(null);

    public static QObjectType QApplication = new(QObject, x => [
        x.Constructor(IntAmpersand["argc"], CharStarStar["argv"]),
        x.Method(Int, "exec"),
        x.Method(null, "exit", Int["returnCode"]),
        ..x.Property(Bool, "quitOnLastWindowClosed")
    ]);

    public static QObjectType QLayout = new(QObject);

    public static QObjectType QWidget = new(QObject, x => [
        x.Constructor(),
        x.Method(null, "show"),
        ..x.Property(QLayout, "layout"),
        ..x.Property(Bool, "isVisible", "setVisible")
    ]);

    public static QObjectType QMainWindow = new(QWidget, x => [
        x.Constructor(),
        ..x.Property(QWidget, "centralWidget"),
        ..x.Property(QSize, "size", "resize"),
        ..x.Property(QString, "windowTitle"),
    ]);

    public static QObjectType QAbstractButton = new(QWidget, x => [
        ..x.Property(QString, "text")
    ]);
    public static QObjectType QPushButton = new(QAbstractButton, x => [
        x.Constructor()
    ]);

    public static QObjectType QFrame = new(QWidget, x => [
        x.Constructor()
    ]);
    public static QObjectType QLabel = new(QFrame, x => [
        x.Constructor(),
        ..x.Property(QString, "text")
    ]);

    public static QObjectType QLineEdit = new(QWidget, x => [
        x.Constructor(),
        ..x.Property(QString, "text"),
        ..x.Property(QString, "placeholderText")
    ]);

    public static QObjectType QAbstractItemModel = new(QObject);
    public static QObjectType QAbstractItemView = new(QFrame, x => [
        ..x.Property(QAbstractItemModel, "model")
    ]);
    public static QObjectType QTreeView = new(QAbstractItemView, x => [
        x.Constructor(),
        ..x.Property(Bool, "rootIsDecorated")
    ]);
    public static QObjectType QGroupBox = new(QWidget, x => [
        x.Constructor(),
        ..x.Property(QString, "title")
    ]);

    public static QObjectType QBoxLayout = new(QLayout, x => [
        x.Constructor(QBoxLayoutDirection["direction"]),
        x.Method(null, "addWidget", QWidget["widget"], Int["stretch"], QtAlignment["alignment"]),
        x.Method(null, "addLayout", QLayout["layout"], Int["stretch"]),
        x.Method(null, "addStretch", Int["stretch"])
    ]);

    public static QObjectType QVBoxLayout = new(QBoxLayout, x => [
        x.Constructor()
    ]);
    public static QObjectType QHBoxLayout = new(QBoxLayout, x => [
        x.Constructor()
    ]);

    public static QObjectType QGridLayout = new(QLayout, x => [
        x.Constructor(),
        x.Method(null, "addWidget", QWidget["widget"], Int["row"], Int["column"], Int["rowSpan"], Int["columnSpan"], QtAlignment["alignment"]),
        x.Method(null, "addLayout", QLayout["layout"], Int["row"], Int["column"], Int["rowSpan"], Int["columnSpan"], QtAlignment["alignment"]),
        x.Method(null, "setColumnMinimumWidth", Int["column"], Int["minSize"]),
        x.Method(null, "setColumnStretch", Int["column"], Int["stretch"]),
        x.Method(null, "setRowMinimumHeight", Int["row"], Int["minSize"]),
        x.Method(null, "setRowStretch", Int["row"], Int["stretch"])
    ]);
    public static QObjectType QFormLayout = new(QLayout, x => [
        x.Constructor(),
        x.Method(null, "addRow", QString["labelText"], QWidget["field"]),
        x.Method(null, "addRow", QString["labelText"], QLayout["field"])
    ]);

    // ===

    public static QObjectType QcAbstractItemModel = new(QAbstractItemModel, x => [
        x.Constructor(VoidStar["handle"], VoidStar["vmt"]),
        x.Method(null, "emitDataChanged", CModelIndex["topLeft"], CModelIndex["buttomRight"]),
        x.Method(VoidStar, "managedHandle")
    ], csName: "QcAbstractItemModel_Unmanaged");

    public static QObjectType QcSignalRouter = new(QObject,x => [
        x.Constructor(QcSignalRouter_RouteDelegate["route"]),
        x.Method(null, "connectSignalCore", QObjectStar["target"], CharStar["signalName"]),
        x.Method(null, "connectEventsCore", QObjectStar["target"])
    ], csName: "QcSignalRouter_Unmanaged");

    public static QObjectType QcSynchronizationContext = new(QObject, x => [
        x.Constructor(QcSynchronizationContext_InvokeDelegate["invoke"]),
        x.Method(null, "postCallback", VoidStar["callback"], VoidStar["state"])
    ], csName: "QcSynchronizationContext_Unmanaged");
}
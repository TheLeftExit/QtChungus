using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

internal static unsafe partial class Qt
{
    internal const string libraryName = "libQtGlue";

    [LibraryImport(libraryName)]
    public static partial void qobject_delete(QObject target);
    [LibraryImport(libraryName)]
    public static partial QObject qapplication_new(int* argc, byte** argv);
    [LibraryImport(libraryName)]
    public static partial void qmainwindow_show(QObject target);
    [LibraryImport(libraryName)]
    public static partial int qapplication_exec(QObject target);
    [LibraryImport(libraryName)]
    public static partial void qcoreapplication_exit(QObject target);
    [LibraryImport(libraryName)]
    public static partial void qabstractitemview_setmodel(QObject target, QObject model);
    [LibraryImport(libraryName)]
    public static partial QObject qabstractitemview_model(QObject target);
    [LibraryImport(libraryName)]
    public static partial void qmainwindow_setcentralwidget(QObject target, QObject widget);
    [LibraryImport(libraryName)]
    public static partial void qboxlayout_addwidget(QObject target, QObject widget, int stretch, QtAlignment alignment);
    [LibraryImport(libraryName)]
    public static partial void qboxlayout_addlayout(QObject target, QObject layout, int stretch);
    [LibraryImport(libraryName)]
    public static partial void qboxlayout_addstretch(QObject target, int stretch);
    [LibraryImport(libraryName)]
    public static partial void qgridlayout_addwidget(QObject target, QObject widget, int row, int column, int rowSpan, int columnSpan, QtAlignment alignment);
    [LibraryImport(libraryName)]
    public static partial void qgridlayout_addlayout(QObject target, QObject layout, int row, int column, int rowSpan, int columnSpan, QtAlignment alignment);
    [LibraryImport(libraryName)]
    public static partial void qformlayout_addwidget(QObject target, ReadOnlySpan<char> labelText, QObject widget);
    [LibraryImport(libraryName)]
    public static partial void qformlayout_addlayout(QObject target, ReadOnlySpan<char> labelText, QObject layout);
}

public enum QtAlignment : int
{
    AlignLeft = 0x0001,
    AlignLeading = AlignLeft,
    AlignRight = 0x0002,
    AlignTrailing = AlignRight,
    AlignHCenter = 0x0004,
    AlignJustify = 0x0008,
    AlignAbsolute = 0x0010,
    AlignHorizontal_Mask = AlignLeft | AlignRight | AlignHCenter | AlignJustify | AlignAbsolute,

    AlignTop = 0x0020,
    AlignBottom = 0x0040,
    AlignVCenter = 0x0080,
    AlignBaseline = 0x0100,
    // Note that 0x100 will clash with Qt::TextSingleLine = 0x100 due to what the comment above
    // this enum declaration states. However, since Qt::AlignBaseline is only used by layouts,
    // it doesn't make sense to pass Qt::AlignBaseline to QPainter::drawText(), so there
    // shouldn't really be any ambiguity between the two overlapping enum values.
    AlignVertical_Mask = AlignTop | AlignBottom | AlignVCenter | AlignBaseline,

    AlignCenter = AlignVCenter | AlignHCenter
};
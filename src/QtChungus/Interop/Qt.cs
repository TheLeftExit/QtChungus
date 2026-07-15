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
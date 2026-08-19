using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace QtChungus.Internal;

public static unsafe partial class Qt
{
    internal const string libraryName = "libQtGlue";

    [LibraryImport(libraryName)]
    public static partial void qobject_delete(QObject target);
    [LibraryImport(libraryName)]
    public static partial QObject qapplication_new(int* argc, byte** argv);
    [LibraryImport(libraryName)]
    public static partial void qwidget_show(QObject target);
    [LibraryImport(libraryName)]
    public static partial int qapplication_exec(QObject target);
    [LibraryImport(libraryName)]
    public static partial void qcoreapplication_exit(QObject target);

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

    [LibraryImport(libraryName)]
    public static partial void qgridlayout_setcolumnminimumwidth(QObject target, int column, int minSize);
    [LibraryImport(libraryName)]
    public static partial void qgridlayout_setcolumnstretch(QObject target, int column, int stretch);
    [LibraryImport(libraryName)]
    public static partial void qgridlayout_setrowminimumheight(QObject target, int row, int minSize);
    [LibraryImport(libraryName)]
    public static partial void qgridlayout_setrowstretch(QObject target, int row, int stretch);
}
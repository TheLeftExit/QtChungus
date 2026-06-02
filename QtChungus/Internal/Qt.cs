using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

public readonly partial record struct QObject(nint Handle);

internal static unsafe partial class Qt
{
    internal const string libraryName = "libQtGlue";

    [LibraryImport(libraryName)]
    public static partial QObject qapplication_new(int* argc, byte** argv);
    [LibraryImport(libraryName)]
    public static partial QObject qmainwindow_new();
    [LibraryImport(libraryName)]
    public static partial void qmainwindow_show(QObject target);
    [LibraryImport(libraryName)]
    public static partial void qmainwindow_delete(QObject target);
    [LibraryImport(libraryName)]
    public static partial int qapplication_exec(QObject target);
    [LibraryImport(libraryName)]
    public static partial void qapplication_delete(QObject target);
    [LibraryImport(libraryName)]
    public static partial QObject qpushbutton_new(QObject parent);
    [LibraryImport(libraryName)]
    public static partial void qabstractbutton_settext(QObject target, ReadOnlySpan<char> text);
    [LibraryImport(libraryName)]
    public static partial char* qabstractbutton_text(QObject target);
    [LibraryImport(libraryName)]
    public static partial void qwidget_setgeometry(QObject target, Rectangle bounds);
    [LibraryImport(libraryName)]
    public static partial Rectangle qwidget_geometry(QObject target);
    [LibraryImport(libraryName)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool qwidget_isvisible(QObject target);
    [LibraryImport(libraryName)]
    public static partial void qguiapplication_setquitonlastwindowclosed(QObject target, [MarshalAs(UnmanagedType.Bool)] bool value);
    [LibraryImport(libraryName)]
    public static partial void qcoreapplication_exit(QObject target);
}
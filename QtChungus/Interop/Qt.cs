using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

internal static unsafe partial class Qt
{
    internal const string libraryName = "libQtGlue";

    [LibraryImport(libraryName)]
    public static partial QObject qapplication_new(int* argc, byte** argv);
    [LibraryImport(libraryName)]
    public static partial void qmainwindow_show(QObject target);
    [LibraryImport(libraryName)]
    public static partial void qmainwindow_delete(QObject target);
    [LibraryImport(libraryName)]
    public static partial int qapplication_exec(QObject target);
    [LibraryImport(libraryName)]
    public static partial void qapplication_delete(QObject target);
    [LibraryImport(libraryName)]
    public static partial void qcoreapplication_exit(QObject target);
}
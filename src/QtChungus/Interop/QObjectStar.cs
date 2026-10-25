using System.ComponentModel;

namespace QtChungus.Internal;

public readonly partial record struct QObjectStar(nint Handle)
{
    internal const string libraryName = "libQtGlue";

    [LibraryImport(libraryName)]
    internal static partial void QcInvoke([MarshalAs(UnmanagedType.LPStr)] string methodId, QObjectStar target, out CVariant returnValue, CVariant* parameters);

    [LibraryImport(libraryName)]
    private static partial void QcDelete(QObjectStar target);
    internal void Dispose() => QcDelete(this);

    [LibraryImport(libraryName)]
    private static partial QObjectStar QcParent(QObjectStar target);
    internal QObjectStar Parent => QcParent(this);

    [LibraryImport(libraryName)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool QcInherits(QObjectStar target, [MarshalAs(UnmanagedType.LPStr)] string className);
    internal bool Inherits(string className) => QcInherits(this, className);
}

using System.ComponentModel;

namespace QtChungus.Internal;

public readonly partial record struct QObjectStar(nint Handle)
{
    internal const string libraryName = "libQtGlue";

    public QObjectStar() : this(0)
    {
        throw new InvalidOperationException("For safety elsewhere in the codebase, this is not allowed. Did you mean `@new`?");
    }

    public static readonly QObjectStar Null = new(0);

    [LibraryImport(libraryName)]
    internal static partial void QcInvoke(int methodId, QObjectStar target, out CVariant returnValue, CVariant* parameters);

    [LibraryImport(libraryName)]
    private static partial void QcDelete(QObjectStar target);
    internal void Dispose() => QcDelete(this);

    [LibraryImport(libraryName)]
    private static partial QObjectStar QcParent(QObjectStar target);
    internal QObjectStar Parent => QcParent(this);

    [LibraryImport(libraryName)]
    [return: MarshalAs(UnmanagedType.U1)]
    private static partial bool QcInherits(QObjectStar target, [MarshalAs(UnmanagedType.LPStr)] string className);
    internal bool IsAssignableTo<T>() where T : struct, IQObject
    {
        return Handle == default || QcInherits(this, T.ClassName);
    }
    internal bool Is<T>(out T result) where T : struct, IQObject
    {
        if (Handle != default && QcInherits(this, T.ClassName))
        {
            result = default(T) with { Handle = this };
            return true;
        }
        result = default;
        return false;
    }
}

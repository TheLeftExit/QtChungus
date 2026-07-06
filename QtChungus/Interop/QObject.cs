using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public readonly unsafe partial record struct QObject(nint Handle)
{
    [LibraryImport(Qt.libraryName)]
    public static partial QObject CreateWidget([MarshalAs(UnmanagedType.LPUTF8Str)] string name, QObject parent);

    [LibraryImport(Qt.libraryName)]
    private static partial QVariant GetPropertyCore(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    public T GetProperty<T>(string propertyName)
    {
        using var variant = GetPropertyCore(this, propertyName);
        return variant.Get<T>();
    }

    [LibraryImport(Qt.libraryName)]
    private static partial void SetPropertyCore(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, QVariant value);
    public void SetProperty<T>(string propertyName, T value)
    {
        var variant = QVariant.Create(value);
        SetPropertyCore(this, propertyName, variant);
    }
}

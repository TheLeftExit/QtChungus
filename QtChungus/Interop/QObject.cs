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
    private static partial void GetPropertyCore(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, out QVariant.TypeCode type, out void* value);
    public T GetProperty<T>(string propertyName) where T : notnull
    {
        GetPropertyCore(this, propertyName, out var type, out var value);
        using var variant = new QVariant(type, value);
        return variant.Get<T>();
    }

    [LibraryImport(Qt.libraryName)]
    private static partial void SetPropertyCore(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, QVariant.TypeCode type, void* value);
    public void SetProperty<T>(string propertyName, T value) where T : notnull
    {
        var variant = QVariant.Create(value);
        SetPropertyCore(this, propertyName, variant.Type, variant.Data);
    }
}

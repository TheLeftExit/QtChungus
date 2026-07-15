using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public readonly unsafe partial record struct QObject(nint Handle)
{
    [LibraryImport(Qt.libraryName)]
    public static partial QObject CreateWidgetCore([MarshalAs(UnmanagedType.LPUTF8Str)] string name, QObject parent);

    [LibraryImport(Qt.libraryName)]
    private static partial CStructPtr<CVariant> GetPropertyCore(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    public T GetProperty<T>(string propertyName)
    {
        // Exception: C# must take ownership of CVariant and its buffer, since it's a one-way C++ to C# transfer
        var pointer = GetPropertyCore(this, propertyName);
        var result = pointer.Target.Get<T>();
        pointer.Target.Dispose();
        pointer.Free();
        return result;
    }

    [LibraryImport(Qt.libraryName)]
    private static partial void SetPropertyCore(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, CStructPtr<CVariant> value);
    public void SetProperty<T>(string propertyName, T value)
    {
        var pointer = CStructPtr<CVariant>.Alloc(CVariant.Create(value));
        SetPropertyCore(this, propertyName, pointer);
    }
}

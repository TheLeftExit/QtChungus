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
    private static partial QVariant* GetPropertyCore(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    public T GetProperty<T>(string propertyName)
    {
        var variant = *GetPropertyCore(this, propertyName);
        var result = variant.Get<T>();
        variant.Dispose(); // Exception : C# must take ownership of the buffer, since it's a one-way C++ to C# transfer
        return result;
    }

    [LibraryImport(Qt.libraryName)]
    private static partial void SetPropertyCore(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, QVariant* value);
    public void SetProperty<T>(string propertyName, T value)
    {
        var variantPtr = (QVariant*)NativeMemory.Alloc((nuint)sizeof(QVariant));
        *variantPtr = QVariant.Create(value);
        SetPropertyCore(this, propertyName, variantPtr);
    }
}

using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QtChungus.Internal;

public interface IQObject
{
    QObject Handle { get; init; }
    static abstract string ClassName { get; }
    void SetDefaults() { }
}

public readonly partial record struct QObject(nint Handle)
{
    [LibraryImport(Qt.libraryName)]
    private static partial QObject CreateWidgetCore([MarshalAs(UnmanagedType.LPUTF8Str)] string name, QObject parent);

    public static T Create<T>(Action<T>? configure = null, QObject parent = default) where T : IQObject, new()
    {
        var handle = CreateWidgetCore(T.ClassName, parent);
        var widget = new T() { Handle = handle };
        widget.SetDefaults();
        if (configure is not null)
        {
            configure(widget);
        }
        return widget;
    }

    [LibraryImport(Qt.libraryName)]
    private static partial CVariant GetPropertyCore(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    [LibraryImport(Qt.libraryName)]
    private static partial QObject GetPropertyCoreEx(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    public T GetProperty<T>(string propertyName)
    {
        if (typeof(T) == typeof(QObject))
        {
            return Unsafe.BitCast<QObject, T>(GetPropertyCoreEx(this, propertyName));
        }
        return GetPropertyCore(this, propertyName).Get<T>();
    }

    [LibraryImport(Qt.libraryName)]
    private static partial void SetPropertyCore(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, CVariant value);
    [LibraryImport(Qt.libraryName)]
    private static partial void SetPropertyCoreEx(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, QObject value);
    public void SetProperty<T>(string propertyName, T value)
    {
        if (typeof(T) == typeof(QObject))
        {
            SetPropertyCoreEx(this, propertyName, Unsafe.BitCast<T, QObject>(value));
        }
        else
        {
            SetPropertyCore(this, propertyName, CVariant.Create(value));
        }

    }
}

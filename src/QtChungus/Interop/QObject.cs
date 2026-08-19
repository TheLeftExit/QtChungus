using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QtChungus.Internal;

public interface IQObject
{
    QObject Handle { get; init; }
    abstract static string ClassName { get; }
    void SetDefaults() { }
}

public readonly partial record struct QObject(nint Handle)
{
    [LibraryImport(Qt.libraryName)]
    private static partial QObject CreateObjectCore([MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    public QObject(string className) : this(CreateObjectCore(className).Handle) { }
    public static TObject Create<TObject>() where TObject : struct, IQObject
    {
        var handle = CreateObjectCore(TObject.ClassName);
        var obj = new TObject { Handle = handle };
        obj.SetDefaults();
        return obj;
    }

    [LibraryImport(Qt.libraryName)]
    private static partial CVariant GetPropertyCore(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name);
    public bool TryGetProperty<T>(string propertyName, out T value)
    {
        if (typeof(T).IsEnum)
        {
            var result = TryGetProperty(propertyName, out int intValue);
            value = Unsafe.BitCast<int, T>(intValue);
            return result;
        }
        Unsafe.SkipInit(out value);
        if (Handle == 0) return false;
        var variant = GetPropertyCore(this, propertyName);
        if (!variant.HasValue) return false;
        value = variant.Get<T>();
        return true;
    }
    public T GetProperty<T>(string propertyName)
    {
        return TryGetProperty<T>(propertyName, out var value) ? value : throw new InvalidOperationException();
    }

    [LibraryImport(Qt.libraryName)]
    private static partial void SetPropertyCore(QObject target, [MarshalAs(UnmanagedType.LPUTF8Str)] string name, CVariant value);
    public void SetProperty<T>(string propertyName, T value)
    {
        if (typeof(T).IsEnum)
        {
            SetProperty(propertyName, Unsafe.BitCast<T, int>(value));
        }
        else
        {
            SetPropertyCore(this, propertyName, CVariant.Create(value));
        }
    }
}

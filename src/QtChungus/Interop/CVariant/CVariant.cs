using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QtChungus.Internal;

public ref partial struct CVariant
{
    private TypeCode Type;
    private fixed long Buffer[4];
    private ref T ValueAs<T>()
    {
        // I really shouldn't need to fix `Value` since we're in a ref struct, but I get CS1666 otherwise.
        fixed (long* value = Buffer) return ref Unsafe.AsRef<T>(value);
    }

    public static TypeCode GetTypeCodeOrThrow<T>()
    {
        return _supportedTypes[typeof(T)];
    }

    public enum TypeCode : int
    {
        UnknownType = 0,
        Bool = 1,
        Int = 2,
        UInt = 3,
        LongLong = 4,
        ULongLong = 5,
        Double = 6,
        QString = 10, // Heap-allocated char32_t*, automatically freed via FreeLater() on allocation, marshaled to `string?`
        QRect = 19,
        QSize = 21,
        QPoint = 25,
        VoidStar = 31, // `nint` used in custom properties
        QObjectStar = 39
    }

    private static readonly Dictionary<Type, TypeCode> _supportedTypes = new() {
        {typeof(bool), TypeCode.Bool},
        {typeof(int), TypeCode.Int},
        {typeof(uint), TypeCode.UInt},
        {typeof(long), TypeCode.LongLong},
        {typeof(ulong), TypeCode.ULongLong},
        {typeof(double), TypeCode.Double},
        {typeof(string), TypeCode.QString},
        {typeof(Rectangle), TypeCode.QRect},
        {typeof(Size), TypeCode.QSize},
        {typeof(Point), TypeCode.QPoint},
        {typeof(nint), TypeCode.VoidStar},
        {typeof(QObjectStar), TypeCode.QObjectStar}
    };

    public static CVariant Create<T>(T value)
    {
        if (typeof(T) == typeof(bool)) return Unsafe.As<T, bool>(ref value);
        if (typeof(T) == typeof(int)) return Unsafe.As<T, int>(ref value);
        if (typeof(T) == typeof(uint)) return Unsafe.As<T, uint>(ref value);
        if (typeof(T) == typeof(long)) return Unsafe.As<T, long>(ref value);
        if (typeof(T) == typeof(ulong)) return Unsafe.As<T, ulong>(ref value);
        if (typeof(T) == typeof(double)) return Unsafe.As<T, double>(ref value);
        if (typeof(T) == typeof(string)) return Unsafe.As<T, string>(ref value);
        if (typeof(T) == typeof(Rectangle)) return Unsafe.As<T, Rectangle>(ref value);
        if (typeof(T) == typeof(Size)) return Unsafe.As<T, Size>(ref value);
        if (typeof(T) == typeof(Point)) return Unsafe.As<T, Point>(ref value);
        if (typeof(T) == typeof(nint)) return Unsafe.As<T, nint>(ref value);
        if (typeof(T) == typeof(QObjectStar)) return Unsafe.As<T, QObjectStar>(ref value);
        throw new NotImplementedException();
    }

    public T Get<T>()
    {
        if (typeof(T) == typeof(bool)) return TryGetValue(out bool value) ? Unsafe.As<bool, T>(ref value) : throw new InvalidOperationException();
        if (typeof(T) == typeof(int)) return TryGetValue(out int value) ? Unsafe.As<int, T>(ref value) : throw new InvalidOperationException();
        if (typeof(T) == typeof(uint)) return TryGetValue(out uint value) ? Unsafe.As<uint, T>(ref value) : throw new InvalidOperationException();
        if (typeof(T) == typeof(long)) return TryGetValue(out long value) ? Unsafe.As<long, T>(ref value) : throw new InvalidOperationException();
        if (typeof(T) == typeof(ulong)) return TryGetValue(out ulong value) ? Unsafe.As<ulong, T>(ref value) : throw new InvalidOperationException();
        if (typeof(T) == typeof(double)) return TryGetValue(out double value) ? Unsafe.As<double, T>(ref value) : throw new InvalidOperationException();
        if (typeof(T) == typeof(string)) return TryGetValue(out string? value) ? Unsafe.As<string, T>(ref value) : throw new InvalidOperationException();
        if (typeof(T) == typeof(Rectangle)) return TryGetValue(out Rectangle value) ? Unsafe.As<Rectangle, T>(ref value) : throw new InvalidOperationException();
        if (typeof(T) == typeof(Size)) return TryGetValue(out Size value) ? Unsafe.As<Size, T>(ref value) : throw new InvalidOperationException();
        if (typeof(T) == typeof(Point)) return TryGetValue(out Point value) ? Unsafe.As<Point, T>(ref value) : throw new InvalidOperationException();
        if (typeof(T) == typeof(nint)) return TryGetValue(out nint value) ? Unsafe.As<nint, T>(ref value) : throw new InvalidOperationException();
        if (typeof(T) == typeof(QObjectStar)) return TryGetValue(out QObjectStar value) ? Unsafe.As<QObjectStar, T>(ref value) : throw new InvalidOperationException();
        throw new NotSupportedException();
    }
}
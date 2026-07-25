using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QtChungus.Internal;

public unsafe ref partial struct CVariant
{
    private TypeCode Type;
    private fixed long Buffer[2];
    private ref T ValueAs<T>()
    {
        // I really shouldn't need to fix `Value` since we're in a ref struct, but I get CS1666 otherwise.
        fixed (long* value = Buffer) return ref Unsafe.AsRef<T>(value);
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
        VoidStar = 31 // `nint` used in custom properties
    }
}
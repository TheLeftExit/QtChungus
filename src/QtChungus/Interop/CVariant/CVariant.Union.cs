using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QtChungus.Internal;

[Union]
public partial struct CVariant : IUnion
{
    public object? Value => throw new NotSupportedException(); // If we're boxing, we're doing something wrong.
    public bool HasValue => Enum.IsDefined(Type) && Type is not TypeCode.UnknownType;

    public CVariant(bool? value)
    {
        if (value is not bool valueCore) return;
        Type = TypeCode.Bool;
        ValueAs<bool>() = valueCore;
    }
    public bool TryGetValue(out bool value)
    {
        value = ValueAs<bool>();
        return Type is TypeCode.Bool;
    }

    public CVariant(int? value)
    {
        if (value is not int valueCore) return;
        Type = TypeCode.Int;
        ValueAs<int>() = valueCore;
    }
    public bool TryGetValue(out int value)
    {
        value = ValueAs<int>();
        return Type is TypeCode.Int;
    }

    public CVariant(uint? value)
    {
        if (value is not uint valueCore) return;
        Type = TypeCode.UInt;
        ValueAs<uint>() = valueCore;
    }
    public bool TryGetValue(out uint value)
    {
        value = ValueAs<uint>();
        return Type is TypeCode.UInt;
    }

    public CVariant(long? value)
    {
        if (value is not long valueCore) return;
        Type = TypeCode.LongLong;
        ValueAs<long>() = valueCore;
    }
    public bool TryGetValue(out long value)
    {
        value = ValueAs<long>();
        return Type is TypeCode.LongLong;
    }

    public CVariant(ulong? value)
    {
        if (value is not ulong valueCore) return;
        Type = TypeCode.ULongLong;
        ValueAs<ulong>() = valueCore;
    }
    public bool TryGetValue(out ulong value)
    {
        value = ValueAs<ulong>();
        return Type is TypeCode.ULongLong;
    }

    public CVariant(double? value)
    {
        if (value is not double valueCore) return;
        Type = TypeCode.Double;
        ValueAs<double>() = valueCore;
    }
    public bool TryGetValue(out double value)
    {
        value = ValueAs<double>();
        return Type is TypeCode.Double;
    }

    public CVariant(string? value)
    {
        Type = TypeCode.QString;
        var buffer = Marshal.StringToHGlobalUni(value);
        async void FreeLater(nint pointer)
        {
            await Task.Yield();
            Marshal.FreeHGlobal(pointer);
        }
        FreeLater(buffer);
        ValueAs<nint>() = buffer;
    }
    public bool TryGetValue(out string? value)
    {
        if (Type is not TypeCode.QString)
        {
            value = null;
            return false;
        }
        value = Marshal.PtrToStringUni(ValueAs<nint>());
        return true;
    }

    public CVariant(Rectangle? value)
    {
        if (value is not Rectangle valueCore) return;
        Type = TypeCode.QRect;
        ValueAs<Rectangle>() = valueCore;
    }
    public bool TryGetValue(out Rectangle value)
    {
        value = ValueAs<Rectangle>();
        return Type is TypeCode.QRect;
    }

    public CVariant(Size? value)
    {
        if (value is not Size valueCore) return;
        Type = TypeCode.QSize;
        ValueAs<Size>() = valueCore;
    }
    public bool TryGetValue(out Size value)
    {
        value = ValueAs<Size>();
        return Type is TypeCode.QSize;
    }

    public CVariant(Point? value)
    {
        if (value is not Point valueCore) return;
        Type = TypeCode.QPoint;
        ValueAs<Point>() = valueCore;
    }
    public bool TryGetValue(out Point value)
    {
        value = ValueAs<Point>();
        return Type is TypeCode.QPoint;
    }

    public CVariant(nint? value)
    {
        if (value is not nint valueCore) return;
        Type = TypeCode.VoidStar;
        ValueAs<nint>() = valueCore;
    }
    public bool TryGetValue(out nint value)
    {
        value = ValueAs<nint>();
        return Type is TypeCode.VoidStar;
    }

    public CVariant(QObjectStar? value)
    {
        if (value is not QObjectStar valueCore) return;
        Type = TypeCode.QObjectStar;
        ValueAs<QObjectStar>() = valueCore;
    }
    public bool TryGetValue(out QObjectStar value)
    {
        value = ValueAs<QObjectStar>();
        return Type is TypeCode.QObjectStar;
    }
}

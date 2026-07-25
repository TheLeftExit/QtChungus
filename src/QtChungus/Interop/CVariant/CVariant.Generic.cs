using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QtChungus.Internal;

public partial struct CVariant
{
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
        {typeof(nint), TypeCode.VoidStar}
    };

    public static TypeCode GetTypeCodeOrThrow<T>()
    {
        return _supportedTypes[typeof(T)];
    }

    private static bool TryEncode<TFrom, TTo>(TFrom source, TypeCode type, out CVariant result)
    {
        if (typeof(TFrom) != typeof(TTo))
        {
            Unsafe.SkipInit(out result);
            return false;
        }
        result = new CVariant() { Type = type };
        result.ValueAs<TTo>() = Unsafe.As<TFrom, TTo>(ref source);
        return true;
    }

    public static CVariant Create<T>(T value)
    {
        CVariant result;
        if (TryEncode<T, bool>(value, TypeCode.Bool, out result)) return result;
        if (TryEncode<T, int>(value, TypeCode.Int, out result)) return result;
        if (TryEncode<T, uint>(value, TypeCode.UInt, out result)) return result;
        if (TryEncode<T, long>(value, TypeCode.LongLong, out result)) return result;
        if (TryEncode<T, ulong>(value, TypeCode.ULongLong, out result)) return result;
        if (TryEncode<T, double>(value, TypeCode.Double, out result)) return result;
        if (typeof(T) == typeof(string))
        {
            var valueString = Unsafe.As<T, string>(ref value);
            var pointer = Marshal.StringToHGlobalUni(valueString);
            SignalRouter.FreeLater(pointer);
            result.Type = TypeCode.QString;
            result.ValueAs<nint>() = pointer;
            return result;
        }
        if (TryEncode<T, Rectangle>(value, TypeCode.QRect, out result)) return result;
        if (TryEncode<T, Size>(value, TypeCode.QSize, out result)) return result;
        if (TryEncode<T, Point>(value, TypeCode.QPoint, out result)) return result;
        if (TryEncode<T, nint>(value, TypeCode.VoidStar, out result)) return result;

        throw new NotImplementedException();
    }

    public T Get<T>()
    {
        switch (Type)
        {
            case TypeCode.UnknownType:
                throw new InvalidOperationException();
            case TypeCode.Bool:
            case TypeCode.Int:
            case TypeCode.UInt:
            case TypeCode.LongLong:
            case TypeCode.ULongLong:
            case TypeCode.Double:
                return ValueAs<T>();
            case TypeCode.QString:
                var resultString = Marshal.PtrToStringUni(ValueAs<nint>());
                return Unsafe.As<string?, T>(ref resultString);
            case TypeCode.QRect:
            case TypeCode.QSize:
            case TypeCode.QPoint:
            case TypeCode.VoidStar:
                return ValueAs<T>();
            default:
                throw new NotImplementedException();
        }
    }
}
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe readonly struct QVariant
{
    public readonly TypeCode Type;
    public readonly void* Data;

    public QVariant(TypeCode type, void* data)
    {
        Type = type;
        Data = data;
    }
    private static bool TryCreate<TFrom, TTo>(TFrom source, out TTo target)
    {
        if (typeof(TFrom) == typeof(TTo))
        {
            target = Unsafe.As<TFrom, TTo>(ref source);
            return true;
        }
        Unsafe.SkipInit(out target);
        return false;
    }
    private static TTo GetCore<TFrom, TTo>(TFrom source)
    {
        if (typeof(TFrom) != typeof(TTo))
        {
            throw new InvalidOperationException();
        }
        return Unsafe.As<TFrom, TTo>(ref source);
    }
    public void Write(TypeCode* type, void** data)
    {
        *type = Type;
        *data = Data;
    }
    /*
    Checklist for new types:
    - TypeCode value
    - Entries in SupportedTypes, Create<T>, Get<T>, Dispose()
    - API: Constructor, GetX, optionally FreeX
    - C++: entries in CVariant
    */

    // https://doc.qt.io/qt-6/qmetatype.html#Type-enum
    public enum TypeCode : int
    {
        UnknownType = 0,
        Bool = 1,
        QString = 10,
        QRect = 19,
        QSize = 21,
        VoidStar = 31
    }
    private static readonly Dictionary<Type, TypeCode> SupportedTypes = new() {
        {typeof(bool), TypeCode.Bool},
        {typeof(string), TypeCode.QString},
        {typeof(Rectangle), TypeCode.QRect},
        {typeof(Size), TypeCode.QSize},
        {typeof(nint), TypeCode.VoidStar}
    };
    public static TypeCode GetTypeCodeOrThrow<T>()
    {
        return SupportedTypes[typeof(T)];
    }

    public static QVariant Create<T>(T value)
    {
        if (TryCreate<T, bool>(value, out var resultBool)) return new(resultBool);
        if (TryCreate<T, string?>(value, out var resultString)) return new(resultString);
        if (TryCreate<T, Rectangle>(value, out var resultRectangle)) return new(resultRectangle);
        if (TryCreate<T, Size>(value, out var resultSize)) return new(resultSize);
        if (TryCreate<T, nint>(value, out var resultIntPtr)) return new(resultIntPtr);
        throw new NotImplementedException();
    }
    public T Get<T>()
    {
        return Type switch
        {
            TypeCode.Bool => GetCore<bool, T>(GetBool()),
            TypeCode.QString => GetCore<string?, T>(GetString()),
            TypeCode.QRect => GetCore<Rectangle, T>(GetRectangle()),
            TypeCode.QSize => GetCore<Size, T>(GetSize()),
            TypeCode.VoidStar => GetCore<nint, T>(GetVoidStar()),
            _ => throw new NotImplementedException()
        };
    }

    public void Dispose() // Only necessary for QObject.GetProperty
    {
        switch (Type)
        {
            case TypeCode.Bool: return;
            case TypeCode.QString: FreeString(); return;
            case TypeCode.QRect: FreeRectangle(); return;
            case TypeCode.QSize: FreeSize(); return;
            case TypeCode.VoidStar: return;
            default: throw new InvalidOperationException();
        }
    }

    // Bool (no Free)
    public QVariant(bool value)
    {
        Type = TypeCode.Bool;
        Data = value ? (void*)0 : (void*)1;
    }
    private bool GetBool()
    {
        return Data != (void*)0;
    }

    // QString
    public QVariant(string? value)
    {
        Type = TypeCode.QString;
        Data = (void*)Marshal.StringToHGlobalUni(value);
    }
    private string? GetString()
    {
        return Marshal.PtrToStringUni((nint)Data)!;
    }
    private void FreeString()
    {
        Marshal.FreeHGlobal((nint)Data);
    }

    // QRectangle
    public QVariant(Rectangle value)
    {
        Type = TypeCode.QRect;
        Data = NativeMemory.AllocZeroed(4, sizeof(int));
        var data = (int*)Data;
        (data[0], data[1], data[2], data[3]) = (value.X, value.Y, value.Width, value.Height);
    }
    private Rectangle GetRectangle()
    {
        var data = (int*)Data;
        return new(data[0], data[1], data[2], data[3]);
    }
    private void FreeRectangle()
    {
        NativeMemory.Free(Data);
    }

    // QSize
    public QVariant(Size value)
    {
        Type = TypeCode.QSize;
        Data = NativeMemory.AllocZeroed(2, sizeof(int));
        var data = (int*)Data;
        (data[0], data[1]) = (value.Width, value.Height);
    }
    private Size GetSize()
    {
        var data = (int*)Data;
        return new(data[0], data[1]);
    }
    private void FreeSize()
    {
        NativeMemory.Free(Data);
    }

    // VoidStar (no Free)
    public QVariant(nint value)
    {
        Type = TypeCode.VoidStar;
        Data = (void*)value;
    }
    public nint GetVoidStar()
    {
        return (nint)Data;
    }
}

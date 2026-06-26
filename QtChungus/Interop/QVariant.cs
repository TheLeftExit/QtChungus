using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public unsafe readonly ref struct QVariant
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
    /*
    Checklist for new types:
    - TypeCode value
    - Entries in Create<T>, Get<T>, Dispose()
    - API: Constructor, GetX, optionally FreeX
    - C++: entries in EncodeQVariant, DecodeQVariant
    */

    // https://doc.qt.io/qt-6/qmetatype.html#Type-enum
    public enum TypeCode : int
    {
        UnknownType = 0,
        Bool = 1,
        QString = 10,
        QRect = 19,
    }
    public static QVariant Create<T>(T value) where T : notnull
    {
        if (TryCreate<T, bool>(value, out var resultBool)) return new(resultBool);
        if (TryCreate<T, string>(value, out var resultString)) return new(resultString);
        if (TryCreate<T, Rectangle>(value, out var resultRectangle)) return new(resultRectangle);
        throw new NotImplementedException();
    }
    public T Get<T>() where T : notnull
    {
        return Type switch
        {
            TypeCode.Bool => GetCore<bool, T>(GetBool()),
            TypeCode.QString => GetCore<string, T>(GetString()),
            TypeCode.QRect => GetCore<Rectangle, T>(GetRectangle()),
            _ => throw new NotImplementedException()
        };
    }
    public void Dispose()
    {
        switch (Type)
        {
            case TypeCode.Bool: return;
            case TypeCode.QString: FreeString(); return;
            case TypeCode.QRect: FreeRectangle(); return;
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
        if (Type is not TypeCode.Bool) throw new InvalidOperationException();
        return Data != (void*)0;
    }

    // QString
    public QVariant(string value)
    {
        Type = TypeCode.QString;
        Data = (void*)Marshal.StringToHGlobalUni(value);
    }
    private string GetString()
    {
        if (Type is not TypeCode.QString) throw new InvalidOperationException();
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
}

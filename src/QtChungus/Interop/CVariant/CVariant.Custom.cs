using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QtChungus.Internal;

public partial struct CVariant
{
    private static void SizeGuard<T>() where T : unmanaged
    {
        var dummyVariant = new CVariant();
        var addressOfVariant = (byte*)&dummyVariant;
        var addressOfBuffer = (byte*)&dummyVariant.Buffer;
        var sizeOfBuffer = sizeof(CVariant) - (addressOfBuffer - addressOfVariant);
        if(sizeof(T) > sizeOfBuffer)
        {
            throw new InvalidOperationException($"Cannot smuggle {typeof(T).Name} in a CVariant! ({sizeof(T)} > {sizeOfBuffer})");
        }
    }

    public static CVariant FromCustomType<T>(T value) where T : unmanaged
    {
        SizeGuard<T>();
        var result = new CVariant();
        result.ValueAs<T>() = value;
        return result;
    }

    public T ToCustomType<T>() where T : unmanaged
    {
        SizeGuard<T>();
        return ValueAs<T>();
    }
}

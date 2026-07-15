using System.Runtime.InteropServices;

public unsafe readonly struct CStructPtr<T> where T : unmanaged, allows ref struct
{
    private readonly T* _pointer;
    public ref T Target => ref *_pointer;

    public CStructPtr(T* pointer) => _pointer = pointer;
    public static CStructPtr<T> Alloc(in T source)
    {
        var pointer = (T*)NativeMemory.Alloc((nuint)sizeof(T));
        *pointer = source;
        return new(pointer);
    }
    public void Free() => NativeMemory.Free(_pointer);

    public static implicit operator T(CStructPtr<T> ptr) => ptr.Target;
}
As a very disagreeable design decision, C++ does not have a standardized way to declare blittable structures. The only safe solution is work with byte buffers manually.

Each `CStruct` class inheritor stores a pointer to a buffer, and implements functions that return typed pointers to chunks of this buffer, mimicking field access, like so:

```cpp
// Hierarchically flattened and simplified C++ class
struct CModelIndex : public CStruct<CModelIndexPtr>
{
    CModelIndex(CModelIndexPtr pointer) : CStruct(pointer) {}

    // As declared in CStruct
    template <typename TField>
    TField *Get(size_t offset) const { return reinterpret_cast<TField *>(pointer + offset); }

    qint32 *row() const { return Get<qint32>(0); }
    qint32 *column() const { return Get<qint32>(sizeof(qint32)); }
    void **internalPointer() const { return Get<void *>(sizeof(qint32) + sizeof(qint32)); }
    qint32 *isValid() const { return Get<qint32>(sizeof(qint32) + sizeof(qint32) + sizeof(void *)); }
};
```

```cs
// C# class whose pointer is a valid CModelIndexPtr
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CModelIndex
{
    public int Row { get; }
    public int Column { get; }
    public nint InternalPointer { get; }
    private int IsValid { get; }
}
```

I considered three different ways to design `CStruct`, all of which have their downsides:

1. All structures are fully packed, C++ uses hardcoded byte offsets for each field.
    - Pros: uncluttered C# structures (`LayoutKind.Sequential, Pack = 1`)
    - Cons: C++ structures need to contain precise offsets for each field, plus the structure size itself. Every field is defined by a sum of up to N `sizeof`-s, which looks ugly and is prone to typos.
2. All structures use 8 bytes per field, C++ uses a "field index" for each field.
    - Pros: C++ fields only contain the field index (0, 1, 2...).
    - Cons: every C# property needs a `[FieldOffset]` with a multiple of 8.
3. All structures are fully packed, C++ uses inferred type templates and pointer arithmetic to deduce the next field's offset based on the return type and return value of the previous field's accessor function.
    - Pros: uncluttered C#, while each C++ field simply references the previous field.
    - Cons:
        - Each C++ field accessor can lead to additional N stack frames,
        - Referencing C++ field accessors within one another is prone to typos,
        - Actually, this solution won't work because pointer arithmetic breaks on function pointers, which are also valid field types for our use case. We get compile-time warnings and segfaults.

For now we're stuck with option 1. I'm hoping that I can come up with a better version of option 3.
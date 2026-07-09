#include <QtGlobal>

#ifndef CSTRUCT_H
#define CSTRUCT_H

template <typename TPtr>
class CStruct
{
private:
    char *const pointer;

protected:
    CStruct(size_t length) : pointer(reinterpret_cast<char *>(malloc(length))) {}
    CStruct(TPtr newPointer) : pointer(reinterpret_cast<char *>(newPointer)) {}

    template <typename TField>
    TField *Get(size_t offset) const
    {
        return reinterpret_cast<TField *>(pointer + offset);
    }

public:
    operator TPtr() const
    {
        return reinterpret_cast<TPtr>(pointer);
    }
    ~CStruct()
    {
        free(pointer);
    }
};
#endif
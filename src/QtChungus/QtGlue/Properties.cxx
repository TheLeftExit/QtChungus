#include <QObject>
#include <QMetaObject>
#include <QMetaType>
#include <QMetaProperty>

#include "CVariant.cxx"

const QMetaProperty FindProperty(QObject *target, char *name)
{
    const QMetaObject *metaObject = target->metaObject();
    int indexOfProperty = metaObject->indexOfProperty(name);
    return metaObject->property(indexOfProperty);
}

extern "C"
{
    CVariantPtr GetPropertyCore(QObject *target, char *name)
    {
        CVariant result = CVariant(FindProperty(target, name).read(target));
        // Exception case where C# must take ownership of the buffer.
        return result.CopyToHeap();
    }

    void SetPropertyCore(QObject *target, char *name, CVariantPtr value)
    {
        FindProperty(target, name).write(target, CVariant(value));
    }
}
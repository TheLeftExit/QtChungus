#include <QObject>
#include <QMetaObject>
#include <QMetaType>
#include <QMetaProperty>

#include "Variant.h"

const QMetaProperty FindProperty(QObject *target, char *name)
{
    const QMetaObject *metaObject = target->metaObject();
    int indexOfProperty = metaObject->indexOfProperty(name);
    return metaObject->property(indexOfProperty);
}

extern "C"
{
    void GetPropertyCore(QObject *target, char *name, QMetaType::Type *type, void **value)
    {
        EncodeQVariant(FindProperty(target, name).read(target), type, value);
    }

    void SetPropertyCore(QObject *target, char *name, QMetaType::Type type, void *value)
    {
        FindProperty(target, name).write(target, DecodeQVariant(type, value));
    }
}
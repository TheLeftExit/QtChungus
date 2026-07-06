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
    QManagedVariant GetPropertyCore(QObject *target, char *name)
    {
        return EncodeQVariant(FindProperty(target, name).read(target));
    }

    void SetPropertyCore(QObject *target, char *name, QManagedVariant value)
    {
        FindProperty(target, name).write(target, DecodeQVariant(value));
    }
}
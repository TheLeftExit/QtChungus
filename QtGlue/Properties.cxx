#include <QObject>
#include <QRect>
#include <QMetaObject>
#include <QMetaType>
#include <QMetaProperty>

const QMetaProperty FindProperty(QObject *target, char *name)
{
    const QMetaObject *metaObject = target->metaObject();
    int indexOfProperty = metaObject->indexOfProperty(name);
    return metaObject->property(indexOfProperty);
}

void EncodeQVariant(QVariant variant, QMetaType::Type *type, void **value)
{
    *type = static_cast<QMetaType::Type>(variant.typeId());
    switch (*type)
    {
    case QMetaType::QString:
    {
        QString string = variant.toString();
        size_t length = (string.size() + 1) * sizeof(char16_t);
        char16_t *buffer = static_cast<char16_t *>(malloc(length));
        memcpy(buffer, string.utf16(), length);
        *value = buffer;
        break;
    }
    case QMetaType::QRect:
    {
        QRect rect = variant.toRect();
        qint32 *buffer = static_cast<qint32 *>(malloc(sizeof(qint32) * 4));
        buffer[0] = rect.x();
        buffer[1] = rect.y();
        buffer[2] = rect.width();
        buffer[3] = rect.height();
        *value = buffer;
        break;
    }
    case QMetaType::Bool:
    {
        *value = reinterpret_cast<void *>(variant.toBool());
        break;
    }
    default:
        qFatal("EncodeQVariant failed!");
    }
}

QVariant DecodeQVariant(QMetaType::Type type, void *value)
{
    switch (type)
    {
    case QMetaType::QString:
    {
        return QVariant(QString((char16_t *)value));
    }
    case QMetaType::QRect:
    {
        qint32 *buffer = (qint32 *)value;
        return QVariant(QRect(buffer[0], buffer[1], buffer[2], buffer[3]));
    }
    case QMetaType::Bool:
    {
        return QVariant(value == nullptr);
    }
    default:
        qFatal("DecodeQVariant failed!");
    }
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
#include <QObject>
#include <QMetaType>
#include <QVariant>
#include <QRect>

#include "Variant.h"

QMetaType::Type GetTypeCode(QVariant variant)
{
    return static_cast<QMetaType::Type>(variant.typeId());
}

QManagedVariant EncodeQVariant(QVariant variant)
{
    qint128 result = 0;
    QMetaType::Type *type = reinterpret_cast<QMetaType::Type *>(&result);
    void **value = reinterpret_cast<void **>(reinterpret_cast<qint64 *>(&result) + 1);

    *type = GetTypeCode(variant);
    switch (*type)
    {
    case QMetaType::QString:
    {
        QString string = variant.toString();
        size_t length = (string.size() + 1) * sizeof(char16_t);
        char16_t *buffer = static_cast<char16_t *>(malloc(length));
        memcpy(buffer, string.utf16(), length);
        *value = buffer;
        return result;
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
        return result;
    }
    case QMetaType::Bool:
    {
        *value = reinterpret_cast<void *>(variant.toBool());
        return result;
    }
    case QMetaType::QSize:{
        QSize size = variant.toSize();
        qint32 *buffer = static_cast<qint32 *>(malloc(sizeof(qint32) * 2));
        buffer[0] = size.width();
        buffer[1] = size.height();
        *value = buffer;
        return result;
    }
    case QMetaType::VoidStar:
    {
        *value = variant.value<void *>();
        return result;
    }
    default:
        qFatal("EncodeQVariant failed!");
    }
}

QVariant DecodeQVariant(QManagedVariant managedVariant)
{
    QMetaType::Type type = *reinterpret_cast<QMetaType::Type *>(&managedVariant);
    void *value = *reinterpret_cast<void **>(reinterpret_cast<qint64 *>(&managedVariant) + 1);

    switch (type)
    {
    case QMetaType::QString:
    {
        QVariant result = QVariant(QString((char16_t *)value));
        free(value);
        return result;
    }
    case QMetaType::QRect:
    {
        qint32 *buffer = (qint32 *)value;
        QVariant result = QVariant(QRect(buffer[0], buffer[1], buffer[2], buffer[3]));
        free(buffer);
        return result;
    }
    case QMetaType::Bool:
    {
        return QVariant(value == nullptr);
    }
    case QMetaType::QSize:
    {
        qint32 *buffer = (qint32 *)value;
        QVariant result = QVariant(QSize(buffer[0], buffer[1]));
        free(buffer);
        return result;
    }
    case QMetaType::VoidStar:
    {
        return QVariant::fromValue(value);
    }
    default:
        qFatal("DecodeQVariant failed!");
    }
}
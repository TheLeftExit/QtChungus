#include "Base.cxx"
#include <QVariant>
#include <QObject>
#include <QMetaType>
#include <QRect>

#ifndef CVARIANT_H
#define CVARIANT_H

typedef void *CVariantPtr;

#define CVARIANT_SIZE (sizeof(qint32) + sizeof(void *))

struct CVariant : public CStruct<CVariantPtr>
{
    CVariant(CVariantPtr pointer) : CStruct(pointer) {}

    QMetaType::Type *type() const
    {
        return reinterpret_cast<QMetaType::Type *>(Get<qint32>(0));
    }
    void **value() const
    {
        return Get<void *>(sizeof(qint32));
    }

    CVariantPtr CopyToHeap()
    {
        CVariantPtr result = malloc(CVARIANT_SIZE);
        memcpy(result, type(), CVARIANT_SIZE);
        return result;
    }

    CVariant(QVariant variant) : CStruct(CVARIANT_SIZE)
    {
        *type() = static_cast<QMetaType::Type>(variant.typeId());
        switch (*type())
        {
        case QMetaType::QString:
        {
            QString string = variant.toString();
            size_t length = (string.size() + 1) * sizeof(char16_t);
            char16_t *buffer = static_cast<char16_t *>(malloc(length));
            memcpy(buffer, string.utf16(), length);
            *value() = buffer;
            return;
        }
        case QMetaType::QRect:
        {
            QRect rect = variant.toRect();
            qint32 *buffer = static_cast<qint32 *>(malloc(sizeof(qint32) * 4));
            buffer[0] = rect.x();
            buffer[1] = rect.y();
            buffer[2] = rect.width();
            buffer[3] = rect.height();
            *value() = buffer;
            return;
        }
        case QMetaType::Bool:
        {
            *value() = reinterpret_cast<void *>(variant.toBool());
            return;
        }
        case QMetaType::QSize:
        {
            QSize size = variant.toSize();
            qint32 *buffer = static_cast<qint32 *>(malloc(sizeof(qint32) * 2));
            buffer[0] = size.width();
            buffer[1] = size.height();
            *value() = buffer;
            return;
        }
        case QMetaType::VoidStar:
        {
            *value() = variant.value<void *>();
            return;
        }
        default:
            qFatal("EncodeQVariant failed!");
        }
    }

    operator QVariant() const
    {
        switch (*type())
        {
        case QMetaType::UnknownType:
        {
            return QVariant();
        }
        case QMetaType::QString:
        {
            QVariant result = QVariant(QString(reinterpret_cast<char16_t *>(*value())));
            return result;
        }
        case QMetaType::QRect:
        {
            qint32 *buffer = reinterpret_cast<qint32 *>(*value());
            QVariant result = QVariant(QRect(buffer[0], buffer[1], buffer[2], buffer[3]));
            return result;
        }
        case QMetaType::Bool:
        {
            return QVariant(*value() == nullptr);
        }
        case QMetaType::QSize:
        {
            qint32 *buffer = reinterpret_cast<qint32 *>(*value());
            QVariant result = QVariant(QSize(buffer[0], buffer[1]));
            return result;
        }
        case QMetaType::VoidStar:
        {
            return QVariant::fromValue(*value());
        }
        default:
            qFatal("DecodeQVariant failed!");
        }
    }
};

#endif
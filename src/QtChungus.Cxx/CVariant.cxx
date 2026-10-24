#include <QVariant>
#include <QRect>
#include <QApplication>
#include <QMetaType>

#include "CModelIndex.cxx"

#ifndef CVARIANT_H
#define CVARIANT_H

struct CVariant
{
    using TypeCode = QMetaType::Type;

private:
    qint32 Type;
    qint64 Value[4];

    template <typename T>
    T *ValueAs()
    {
        return reinterpret_cast<T *>(Value);
    }

    template <typename T>
    T GetValue(TypeCode typeCode)
    {
        if (Type != typeCode)
        {
            qFatal("Attempted to cast CVariant containing %s to %s", QMetaType(Type).name(), QMetaType(typeCode).name());
        }
        return *ValueAs<T>();
    }

public:
    CVariant(bool value)
    {
        Type = TypeCode::Bool;
        *ValueAs<bool>() = value;
    }
    operator bool()
    {
        return GetValue<bool>(TypeCode::Bool);
    }

    CVariant(qint32 value)
    {
        Type = TypeCode::Int;
        *ValueAs<qint32>() = value;
    }
    operator qint32()
    {
        return GetValue<qint32>(TypeCode::Int);
    }

    CVariant(quint32 value)
    {
        Type = TypeCode::UInt;
        *ValueAs<quint32>() = value;
    }
    operator quint32()
    {
        return GetValue<quint32>(TypeCode::UInt);
    }

    CVariant(qint64 value)
    {
        Type = TypeCode::LongLong;
        *ValueAs<qint64>() = value;
    }
    operator qint64()
    {
        return GetValue<qint64>(TypeCode::LongLong);
    }

    CVariant(quint64 value)
    {
        Type = TypeCode::ULongLong;
        *ValueAs<quint64>() = value;
    }
    operator quint64()
    {
        return GetValue<quint64>(TypeCode::ULongLong);
    }

    CVariant(double value)
    {
        Type = TypeCode::Double;
        *ValueAs<double>() = value;
    }
    operator double()
    {
        return GetValue<double>(TypeCode::Double);
    }

    CVariant(QString value)
    {
        Type = TypeCode::QString;
        char16_t *buffer = static_cast<char16_t *>(calloc(value.length() + 1, sizeof(char16_t)));
        memcpy(buffer, value.utf16(), value.length() * sizeof(char16_t));
        QMetaObject::invokeMethod(qApp, [buffer]
                                  { free(buffer); }, Qt::QueuedConnection);
        *ValueAs<char16_t *>() = buffer;
    }
    operator QString()
    {
        char16_t *buffer = GetValue<char16_t *>(TypeCode::QString);
        return QString(buffer);
    }

    CVariant(QRect value)
    {
        Type = TypeCode::QRect;
        ValueAs<qint32>()[0] = value.x();
        ValueAs<qint32>()[1] = value.y();
        ValueAs<qint32>()[2] = value.width();
        ValueAs<qint32>()[3] = value.height();
    }
    operator QRect()
    {
        GetValue<qint32>(TypeCode::QRect);
        return QRect(
            ValueAs<qint32>()[0],
            ValueAs<qint32>()[1],
            ValueAs<qint32>()[2],
            ValueAs<qint32>()[4]);
    }

    CVariant(QSize value)
    {
        Type = TypeCode::QSize;
        ValueAs<qint32>()[0] = value.width();
        ValueAs<qint32>()[1] = value.height();
    }
    operator QSize()
    {
        GetValue<qint32>(TypeCode::QSize);
        return QSize(
            ValueAs<qint32>()[0],
            ValueAs<qint32>()[1]);
    }

    CVariant(QPoint value)
    {
        Type = TypeCode::QPoint;
        ValueAs<qint32>()[0] = value.x();
        ValueAs<qint32>()[1] = value.y();
    }
    operator QPoint()
    {
        GetValue<qint32>(TypeCode::QPoint);
        return QPoint(
            ValueAs<qint32>()[0],
            ValueAs<qint32>()[1]);
    }

    CVariant(void *value)
    {
        Type = TypeCode::VoidStar;
        *ValueAs<void *>() = value;
    }
    operator void *()
    {
        return GetValue<void *>(TypeCode::VoidStar);
    }

    CVariant(QObject *value)
    {
        Type = TypeCode::QObjectStar;
        *ValueAs<QObject *>() = value;
    }
    operator QObject *()
    {
        return GetValue<QObject *>(TypeCode::QObjectStar);
    }

public:
#define QTGLUE_FOR_EACH_CVARIANT_TYPE(F) \
    F(TypeCode::Bool, bool)              \
    F(TypeCode::Int, qint32)             \
    F(TypeCode::UInt, quint32)           \
    F(TypeCode::LongLong, qint64)        \
    F(TypeCode::ULongLong, quint64)      \
    F(TypeCode::Double, double)          \
    F(TypeCode::QString, QString)        \
    F(TypeCode::QRect, QRect)            \
    F(TypeCode::QSize, QSize)            \
    F(TypeCode::QPoint, QPoint)          \
    F(TypeCode::VoidStar, void *)        \
    F(TypeCode::QObjectStar, QObject *)

    CVariant(QVariant source)
    {
        switch (source.typeId())
        {

#define TRY_RETURN(TYPE_CODE, TYPE)             \
    case TYPE_CODE:                             \
        *this = CVariant(source.value<TYPE>()); \
        return;
            QTGLUE_FOR_EACH_CVARIANT_TYPE(TRY_RETURN)
#undef TRY_RETURN

        case TypeCode::UnknownType:
            Type = TypeCode::UnknownType;
            return;
        }

        if (QMetaType(source.typeId()).flags().testFlag(QMetaType::PointerToQObject))
        {
            Type = TypeCode::QObjectStar;
            *ValueAs<QObject *>() = source.value<QObject *>();
            return;
        }

        qFatal("Failed to serialize CVariant with unknown typeId: %d (%s)", source.typeId(), QMetaType(source.typeId()).name());
    }

    operator QVariant()
    {
        switch (Type)
        {

#define TRY_RETURN(TYPE_CODE, TYPE) \
    case TYPE_CODE:                 \
        return QVariant::fromValue<TYPE>(*this);
            QTGLUE_FOR_EACH_CVARIANT_TYPE(TRY_RETURN)
#undef TRY_RETURN

        case QMetaType::UnknownType:
            return QVariant();
        }

        qFatal("Failed to deserialize CVariant with unknown typeId: %d (%s)", Type, QMetaType(Type).name());
    }

public:
    template <typename T>
    static CVariant FromCustomType(T value)
    {
        CVariant result = CVariant(QVariant());
        *result.ValueAs<T>() = value;
        return result;
    }
    template <typename T>
    T ToCustomType()
    {
        return *ValueAs<T>();
    }
};

#endif
#include <QtGlobal>
#include <QVariant>
#include <QRect>

extern "C" void FreeLater(void *pointer);

// Thanks to Qt for the meta-macro idea
#define QTGLUE_FOR_EACH_SIMPLE_QVARIANT_TYPE(F) \
    F(QMetaType::Bool, bool)                    \
    F(QMetaType::Int, int)                      \
    F(QMetaType::UInt, uint)                    \
    F(QMetaType::LongLong, qlonglong)           \
    F(QMetaType::ULongLong, qulonglong)         \
    F(QMetaType::Double, double)                \
    /* QString */                               \
    F(QMetaType::QRect, QRect)                  \
    F(QMetaType::QSize, QSize)                  \
    F(QMetaType::QPoint, QPoint)                \
    F(QMetaType::VoidStar, void *)

struct CRectangle
{
    qint32 X, Y, Width, Height;
    CRectangle(QRect source) : X(source.x()), Y(source.y()), Width(source.width()), Height(source.height()) {}
    operator QRect() const
    {
        return QRect(X, Y, Width, Height);
    }
};

struct CPoint
{
    qint32 X, Y;
    CPoint(QPoint source) : X(source.x()), Y(source.y()) {}
    operator QPoint() const
    {
        return QPoint(X, Y);
    }
    CPoint(QSize source) : X(source.width()), Y(source.height()) {}
    operator QSize() const
    {
        return QSize(X, Y);
    }
};

struct CVariant
{
    qint32 Type;
    qint64 Value[2];

    template <typename T>
    T *ValueAs()
    {
        return reinterpret_cast<T *>(Value);
    }

    CVariant(QVariant source)
    {
        Type = source.typeId();
        switch (static_cast<QMetaType::Type>(Type))
        {

#define TRY_RETURN(QMETATYPE, CTYPE)               \
    case QMETATYPE:                                \
        *ValueAs<CTYPE>() = source.value<CTYPE>(); \
        break;
            QTGLUE_FOR_EACH_SIMPLE_QVARIANT_TYPE(TRY_RETURN)
#undef TRY_RETURN

        case QMetaType::QString:
        {
            QString string = source.toString();
            size_t length = (string.size() + 1) * sizeof(char16_t);
            char16_t *buffer = static_cast<char16_t *>(malloc(length));
            memcpy(buffer, string.utf16(), length);
            *ValueAs<char16_t *>() = buffer;
            FreeLater(buffer);
            break;
        }
        case QMetaType::UnknownType:
            break;
        default:
            qFatal("Failed to serialize QVariant with unknown typeId: %d", Type);
            break;
        }
    }

    operator QVariant()
    {
        switch (static_cast<QMetaType::Type>(Type))
        {

#define TRY_RETURN(QMETATYPE, CTYPE) \
    case QMETATYPE:                  \
        return QVariant::fromValue<CTYPE>(*ValueAs<CTYPE>());
            QTGLUE_FOR_EACH_SIMPLE_QVARIANT_TYPE(TRY_RETURN)
#undef TRY_RETURN

        case QMetaType::QString:
            return QVariant(QString(*ValueAs<char16_t *>()));
        case QMetaType::UnknownType:
            return QVariant();
        default:
            qFatal("Failed to deserialize QVariant with unknown typeId: %d", Type);
            break;
        }
    }
};
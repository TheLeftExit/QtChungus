#include <QObject>
#include <QRect>
#include <QMetaObject>
#include <QMetaProperty>

const QMetaProperty FindProperty(QObject *target, char *name)
{
    const QMetaObject *metaObject = target->metaObject();
    int indexOfProperty = metaObject->indexOfProperty(name);
    return metaObject->property(indexOfProperty);
}

extern "C"
{
    const ushort *GetProperty_string(QObject *target, char *name) { return FindProperty(target, name).read(target).toString().utf16(); }
    void SetProperty_string(QObject *target, char *name, char16_t *value) { FindProperty(target, name).write(target, QString(value)); }

    qint64 GetProperty_int64(QObject *target, char *name) { return FindProperty(target, name).read(target).toLongLong(); }
    void SetProperty_int64(QObject *target, char *name, qint64 value) { FindProperty(target, name).write(target, value); }

    void GetProperty_rect(QObject *target, char *name, qint32 *buffer)
    {
        QRect result = FindProperty(target, name).read(target).toRect();
        buffer[0] = result.x();
        buffer[1] = result.y();
        buffer[2] = result.width();
        buffer[3] = result.height();
    }
    void SetProperty_rect(QObject *target, char *name, qint32 *buffer)
    {
        QRect value = QRect(buffer[0], buffer[1], buffer[2], buffer[3]);
        FindProperty(target, name).write(target, value);
    }
}
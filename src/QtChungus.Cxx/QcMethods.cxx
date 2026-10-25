#include <QObject>

extern "C" void QcDelete(QObject *target)
{
    delete target;
}

extern "C" QObject *QcParent(QObject *target)
{
    return target->parent();
}

extern "C" bool QcInherits(QObject *target, const char *className)
{
    return !target || target->inherits(className);
}
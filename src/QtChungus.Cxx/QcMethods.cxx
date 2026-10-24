#include <QObject>

extern "C" void QcDelete(QObject *target)
{
    delete target;
}

extern "C" QObject *QcParent(QObject *target)
{
    return target->parent();
}
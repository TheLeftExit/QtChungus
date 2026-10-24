#include <QObject>
#include <QMetaObject>
#include <QApplication>

#ifndef QCSYNCHRONIZATIONCONTEXT_H
#define QCSYNCHRONIZATIONCONTEXT_H

class QcSynchronizationContext : public QObject
{
    Q_OBJECT
public:
    QcSynchronizationContext(void (*invoke)(void *, void *)) : m_invoke(invoke) {}
    void PostCallback(void *arg1, void *arg2)
    {
        QMetaObject::invokeMethod(
            this,
            "InvokeCallback",
            Qt::QueuedConnection,
            Q_ARG(void *, arg1),
            Q_ARG(void *, arg2));
    }

private:
    void (*m_invoke)(void *, void *);

public slots:
    void InvokeCallback(void *arg1, void *arg2)
    {
        m_invoke(arg1, arg2);
    }
};

#endif
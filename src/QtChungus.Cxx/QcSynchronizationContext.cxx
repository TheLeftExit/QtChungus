#include <QObject>
#include <QMetaObject>
#include <QApplication>

class QcSynchronizationContext : public QObject
{
    Q_OBJECT
public:
    QcSynchronizationContext(void (*invoke)(void *, void *)) : m_invoke(invoke) {}

private:
    void (*m_invoke)(void *, void *);

public slots:
    void InvokeCallback(void *arg1, void *arg2)
    {
        m_invoke(arg1, arg2);
    }
};

extern "C"
{
    QcSynchronizationContext *CreateSynchronizationContext(void (*invoke)(void *, void *))
    {
        return new QcSynchronizationContext(invoke);
    }

    void PostCallback(void *arg1, void *arg2, QcSynchronizationContext *context)
    {
        QMetaObject::invokeMethod(
            context,
            "InvokeCallback",
            Qt::QueuedConnection,
            Q_ARG(void *, arg1),
            Q_ARG(void *, arg2));
    }
}

#include "QcSynchronizationContext.moc"
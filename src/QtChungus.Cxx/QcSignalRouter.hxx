#include <QObject>
#include <QMetaMethod>
#include <QWidget>
#include <QEvent>

#ifndef QCSIGNALROUTER_H
#define QCSIGNALROUTER_H

extern "C" void InvokeCallbackCore(void *callbackHandle, void *stateHandle);

class QcSignalRouter : public QObject
{
    Q_OBJECT
public:
    QcSignalRouter(void (*route)(QObject *, const char *)) : m_route(route) {}
    void connectSignalCore(QObject *target, char *signalName)
    {
        QMetaMethod signal = target->metaObject()->method(target->metaObject()->indexOfMethod(signalName));
        int slotIndex = QcSignalRouter::staticMetaObject.indexOfSlot("RouteSignal()");
        QMetaMethod slot = QcSignalRouter::staticMetaObject.method(slotIndex);
        QObject::connect(
            target,
            signal,
            this,
            slot,
            static_cast<Qt::ConnectionType>(Qt::ConnectionType::DirectConnection | Qt::ConnectionType::UniqueConnection));
    }
    void connectEventsCore(QObject *target)
    {
        target->installEventFilter(this);
    }

private:
    void (*m_route)(QObject *, const char *);

public slots:
    void RouteSignal()
    {
        QObject *sender = QObject::sender();
        QMetaMethod method = sender->metaObject()->method(QObject::senderSignalIndex());
        const char *signalName = method.name().constData();
        m_route(sender, signalName);
    }

protected:
    bool eventFilter(QObject *obj, QEvent *ev) override
    {
        switch (ev->type())
        {
        case QEvent::Close:
            m_route(obj, "QEvent::Close");
            break;
        }
        return false;
    }
};

#endif
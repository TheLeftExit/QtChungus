#include <QObject>
#include <QMetaMethod>
#include <QWidget>
#include <QEvent>

extern "C" void RouteSignalCore(QObject *sender, const char *signalName);
extern "C" void InvokeCallbackCore(void *callbackHandle, void *stateHandle);

class SignalRouter : public QObject
{
    Q_OBJECT
public:
    static SignalRouter *Instance;
public slots:
    void RouteSignal()
    {
        QObject *sender = QObject::sender();
        QMetaMethod method = sender->metaObject()->method(QObject::senderSignalIndex());
        const char *signalName = method.name().constData();
        // const char *senderName = method.enclosingMetaObject()->className();
        RouteSignalCore(sender, signalName);
    }
    void InvokeCallback(void *callbackHandle, void *stateHandle)
    {
        InvokeCallbackCore(callbackHandle, stateHandle);
    }

protected:
    bool eventFilter(QObject *obj, QEvent *ev) override
    {
        switch (ev->type())
        {
        case QEvent::Close:
            RouteSignalCore(obj, "QEvent::Close");
            break;
        }
        return false;
    }
};

SignalRouter *SignalRouter::Instance;

extern "C"
{
    void InitializeRouter()
    {
        SignalRouter::Instance = new SignalRouter();
    }
    void ConnectSignal(QObject *target, char *signalName)
    {
        QMetaMethod signal = target->metaObject()->method(target->metaObject()->indexOfMethod(signalName));
        /*
        if(!signal.isValid()){
            std::cout << "Signal not found! Possible signals:" << std::endl;
            for(int i = 0; i < target->metaObject()->methodCount(); i++){
                std::cout << target->metaObject()->method(i).methodSignature().toStdString() << std::endl;
            }
        }
        */
        int slotIndex = SignalRouter::staticMetaObject.indexOfSlot("RouteSignal()");
        QMetaMethod slot = SignalRouter::staticMetaObject.method(slotIndex);
        QObject::connect(
            target,
            signal,
            SignalRouter::Instance,
            slot,
            static_cast<Qt::ConnectionType>(Qt::ConnectionType::DirectConnection | Qt::ConnectionType::UniqueConnection));
    }
    void ConnectEvents(QObject *target)
    {
        target->installEventFilter(SignalRouter::Instance);
    }
    QObject *GetParent(QObject *target)
    {
        return target->parent();
    }
    void PostCallback(void *callbackHandle, void *stateHandle)
    {
        QMetaObject::invokeMethod(
            SignalRouter::Instance,
            "InvokeCallback",
            Qt::ConnectionType::QueuedConnection,
            Q_ARG(void *, callbackHandle),
            Q_ARG(void *, stateHandle));
    }
}

#include "SignalRouter.moc"
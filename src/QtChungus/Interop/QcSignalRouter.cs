using System.Runtime.InteropServices;
using System.Text;

namespace QtChungus.Internal;

public interface ISignalReceiver
{
    void ProcessSignal(QObject sender, ReadOnlySpan<char> signalName);
}

public unsafe static partial class QcSignalRouter
{
    [LibraryImport(Qt.libraryName, StringMarshalling = StringMarshalling.Utf8)]
    private static partial void ConnectSignalCore(QObject target, string signalName, nint router);
    [LibraryImport(Qt.libraryName)]
    private static partial void ConnectEventsCore(QObject target, nint router);
    [LibraryImport(Qt.libraryName)]
    private static partial nint CreateSignalRouter(delegate* unmanaged<QObject, byte*, void> route);

    private static Lazy<nint> _router = new(() => CreateSignalRouter(&RouteSignalCore));
    public static void ConnectSignal(QObject target, string signalName) => ConnectSignalCore(target, signalName, _router.Value);
    public static void ConnectEvents(QObject target) => ConnectEventsCore(target, _router.Value);

    [UnmanagedCallersOnly]
    private static void RouteSignalCore(QObject sender, byte* signalName)
    {
        for (var nextSender = sender; nextSender.Handle != 0; nextSender = Qt.qobject_parent(nextSender))
        {
            if (_subscribers.TryGetValue(nextSender, out var subscribers))
            {
                var signalNameUtf8 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(signalName);
                Span<char> signalNameUtf16 = stackalloc char[Encoding.UTF8.GetCharCount(signalNameUtf8)];
                Encoding.UTF8.GetChars(signalNameUtf8, signalNameUtf16);
                foreach (var subscriber in subscribers)
                {
                    subscriber.ProcessSignal(sender, signalNameUtf16);
                }
                return;
            }
        }
        throw new InvalidOperationException("We received a signal from a QObject without an ISignalReceiver parent!");
    }

    private static Dictionary<QObject, List<ISignalReceiver>> _subscribers = new();
    public static IDisposable Subscribe(QObject target, ISignalReceiver signalReceiver)
    {
        if (!_subscribers.TryGetValue(target, out var subscribers))
        {
            subscribers = _subscribers[target] = new();
        }
        subscribers.Add(signalReceiver);
        return new Subscription(target, signalReceiver);
    }
    private class Subscription : IDisposable
    {
        private QObject _target;
        private ISignalReceiver _signalReceiver;
        public Subscription(QObject target, ISignalReceiver signalReceiver)
        {
            _target = target;
            _signalReceiver = signalReceiver;
        }
        public void Dispose()
        {
            var list = _subscribers[_target];
            list.Remove(_signalReceiver);
            if (list.Count == 0)
            {
                _subscribers.Remove(_target);
            }
        }
    }
}

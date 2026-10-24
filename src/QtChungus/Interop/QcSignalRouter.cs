using System.Runtime.InteropServices;
using System.Text;

namespace QtChungus.Internal;

public interface ISignalReceiver
{
    void ProcessSignal(QObjectStar sender, ReadOnlySpan<char> signalName);
}

public unsafe static partial class QcSignalRouter
{
    [LibraryImport(QObject.libraryName, StringMarshalling = StringMarshalling.Utf8)]
    private static partial void ConnectSignalCore(QObjectStar target, string signalName, nint router);
    [LibraryImport(QObject.libraryName)]
    private static partial void ConnectEventsCore(QObjectStar target, nint router);
    [LibraryImport(QObject.libraryName)]
    private static partial nint CreateSignalRouter(delegate* unmanaged<QObjectStar, byte*, void> route);

    private static Lazy<nint> _router = new(() => CreateSignalRouter(&RouteSignalCore));
    public static void ConnectSignal(QObjectStar target, string signalName) => ConnectSignalCore(target, signalName, _router.Value);
    public static void ConnectEvents(QObjectStar target) => ConnectEventsCore(target, _router.Value);

    [UnmanagedCallersOnly]
    private static void RouteSignalCore(QObjectStar sender, byte* signalName)
    {
        var signalNameUtf8 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(signalName);

        var parentSubscriberFound = false;
        for (var nextSender = sender; nextSender.Handle != 0; nextSender = nextSender.Parent)
        {
            if (_subscribers.TryGetValue(nextSender, out var subscribers))
            {
                Span<char> signalNameUtf16 = stackalloc char[Encoding.UTF8.GetCharCount(signalNameUtf8)];
                Encoding.UTF8.GetChars(signalNameUtf8, signalNameUtf16);
                foreach (var subscriber in subscribers)
                {
                    subscriber.ProcessSignal(sender, signalNameUtf16);
                }
                break;
            }
        }

        if (!parentSubscriberFound)
        {
            throw new InvalidOperationException("We received a signal from a QObject without an ISignalReceiver parent!");
        }
    }

    private static Dictionary<QObjectStar, List<ISignalReceiver>> _subscribers = new();
    public static IDisposable Subscribe(QObjectStar target, ISignalReceiver signalReceiver)
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
        private QObjectStar _target;
        private ISignalReceiver _signalReceiver;
        public Subscription(QObjectStar target, ISignalReceiver signalReceiver)
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

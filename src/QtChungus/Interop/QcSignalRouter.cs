using System.Runtime.InteropServices;
using System.Text;

namespace QtChungus.Internal;

public interface ISignalReceiver
{
    void ProcessSignal(QObjectStar sender, ReadOnlySpan<char> signalName);
}

public unsafe static partial class QcSignalRouter
{
    private static Lazy<QcSignalRouter_Unmanaged> _router = new(() => new(&RouteSignalCore));
    public static void ConnectSignal(QObjectStar target, string signalName) => _router.Value.ConnectSignal(target, signalName);
    public static void ConnectEvents(QObjectStar target) => _router.Value.ConnectEvents(target);

    [UnmanagedCallersOnly]
    private static void RouteSignalCore(QObjectStar sender, byte* signalName)
    {
        var signalNameUtf8 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(signalName);
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
                return;
            }
        }
        throw new InvalidOperationException("We received a signal from a QObject without an ISignalReceiver parent!");
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

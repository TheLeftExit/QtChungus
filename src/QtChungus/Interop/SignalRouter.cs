using System.Runtime.InteropServices;
using System.Text;

namespace QtChungus.Internal;

public interface ISignalReceiver
{
    void ProcessSignal(QObject sender, ReadOnlySpan<char> signalName);
}

public unsafe partial class SignalRouter : ICReversePInvokeShimInitializer
{
    private static Dictionary<QObject, List<ISignalReceiver>> _subscribers = new();

    private SignalRouter() { }

    [UnmanagedCallersOnly(EntryPoint = "RouteSignalCore")]
    private static void RouteSignalCore(QObject sender, byte* signalName)
    {
        for (var nextSender = sender; nextSender.Handle != 0; nextSender = GetParent(nextSender))
        {
            if (_subscribers.TryGetValue(nextSender, out var subscribers))
            {
                var signalNameUtf8 = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(signalName);
                Span<char> signalNameUtf16 = stackalloc char[Encoding.UTF8.GetCharCount(signalNameUtf8)];
                Encoding.UTF8.GetChars(signalNameUtf8, signalNameUtf16);
                //Console.WriteLine(signalNameUtf16);
                foreach (var subscriber in subscribers)
                {
                    subscriber.ProcessSignal(sender, signalNameUtf16);
                }
                return;
            }
        }
        throw new InvalidOperationException("We received a signal from a QObject without an ISignalReceiver parent!");
    }
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

    [LibraryImport(Qt.libraryName)]
    private static partial QObject GetParent(QObject target);

    [LibraryImport(Qt.libraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial void ConnectSignal(QObject target, string signalName);
    [LibraryImport(Qt.libraryName)]
    public static partial void ConnectEvents(QObject target);
    [LibraryImport(Qt.libraryName)]
    public static partial void FreeLater(nint pointer);

    static void ICReversePInvokeShimInitializer.Initialize(ref CReversePInvokeShim shim)
    {
        shim.RouteSignalCore = &RouteSignalCore;
    }
}

public partial class QtSynchronizationContext : SynchronizationContext, ICReversePInvokeShimInitializer
{
    public override void Post(SendOrPostCallback d, object? state)
    {
        PostCallback((nint)GCHandle.Alloc(d), (nint)GCHandle.Alloc(state));
    }

    [LibraryImport(Qt.libraryName)]
    private static partial void PostCallback(nint callbackHandle, nint stateHandle);

    [UnmanagedCallersOnly(EntryPoint = "InvokeCallbackCore")]
    private static void InvokeCallbackCore(nint callbackHandle, nint stateHandle)
    {
        var callbackGcHandle = GCHandle.FromIntPtr(callbackHandle);
        var stateGcHandle = GCHandle.FromIntPtr(stateHandle);
        ((SendOrPostCallback)callbackGcHandle.Target!)(stateGcHandle.Target!);
        callbackGcHandle.Free();
        stateGcHandle.Free();
    }

    unsafe static void ICReversePInvokeShimInitializer.Initialize(ref CReversePInvokeShim shim)
    {
        shim.InvokeCallbackCore = &InvokeCallbackCore;
    }
}
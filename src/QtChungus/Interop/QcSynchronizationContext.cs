using System.Runtime.InteropServices;

namespace QtChungus.Internal;

public unsafe partial class QcSynchronizationContext : SynchronizationContext, IDisposable
{
    private readonly QcSynchronizationContext_Unmanaged _context;

    public QcSynchronizationContext()
    {
        _context = new(&InvokeCallbackCore);
    }

    [UnmanagedCallersOnly]
    private static void InvokeCallbackCore(nint callbackHandle, nint stateHandle)
    {
        var callbackGcHandle = GCHandle.FromIntPtr(callbackHandle);
        var stateGcHandle = GCHandle.FromIntPtr(stateHandle);
        ((SendOrPostCallback)callbackGcHandle.Target!)(stateGcHandle.Target!);
        callbackGcHandle.Free();
        stateGcHandle.Free();
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        _context.PostCallback((nint)GCHandle.Alloc(d), (nint)GCHandle.Alloc(state));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
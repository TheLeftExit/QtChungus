using System.Runtime.InteropServices;

namespace QtChungus.Internal;

public unsafe partial class QcSynchronizationContext : SynchronizationContext
{
    [LibraryImport(Qt.libraryName)]
    private static partial void PostCallback(nint callbackHandle, nint stateHandle, nint context);
    [LibraryImport(Qt.libraryName)]
    private static partial nint CreateSynchronizationContext(delegate *unmanaged<nint, nint, void> invoke);
    
    private nint _context;

    public QcSynchronizationContext()
    {
        _context = CreateSynchronizationContext(&InvokeCallbackCore);
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
        PostCallback((nint)GCHandle.Alloc(d), (nint)GCHandle.Alloc(state), _context);
    }

    

    
}
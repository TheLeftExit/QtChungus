using System.Runtime.InteropServices;

interface ICReversePInvokeShimInitializer
{
    static abstract void Initialize(ref CReversePInvokeShim shim);
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
unsafe partial struct CReversePInvokeShim
{
#if QTGLUE_SHARED
    private static void InitializeFrom<T>(ref CReversePInvokeShim shim) where T : ICReversePInvokeShimInitializer
    {
        T.Initialize(ref shim);
    }
    [LibraryImport(Qt.libraryName)]
    private static partial void InitializeShim(CReversePInvokeShim* shim);
    public static void Initialize()
    {
        var shimPtr = (CReversePInvokeShim*)NativeMemory.AllocZeroed((nuint)sizeof(CReversePInvokeShim));
        InitializeFrom<SignalRouter>(ref *shimPtr);
        InitializeFrom<QtSynchronizationContext>(ref *shimPtr);
        InitializeFrom<QAbstractItemModel>(ref *shimPtr);
        InitializeShim(shimPtr);
    }
#endif
    public delegate* unmanaged<QObject, byte*, void> RouteSignalCore;
    public delegate* unmanaged<nint, nint, void> InvokeCallbackCore;
    public delegate* unmanaged<nint, CModelIndex*, int> qmodel_rowcount;
    public delegate* unmanaged<nint, CModelIndex*, QtItemDataRole, QVariant*> qmodel_data;
    public delegate* unmanaged<nint, int, QtOrientation, QtItemDataRole, QVariant*> qmodel_headerdata;
    public delegate* unmanaged<nint, CModelIndex*, int> qmodel_columncount;
    public delegate* unmanaged<nint, int, int, CModelIndex*, CModelIndex*> qmodel_index;
    public delegate* unmanaged<nint, CModelIndex*, CModelIndex*> qmodel_parent;
    public delegate* unmanaged<nint, CModelIndex*, QtItemFlag> qmodel_flags;
    public delegate* unmanaged<nint, CModelIndex*, QVariant*, QtItemDataRole, int> qmodel_setdata;
}


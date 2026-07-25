using System.Runtime.InteropServices;

namespace QtChungus.Internal;

interface ICReversePInvokeShimInitializer
{
    static abstract void Initialize(ref CReversePInvokeShim shim);
}

unsafe partial struct CReversePInvokeShim
{
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
    public delegate* unmanaged<QObject, byte*, void> RouteSignalCore;
    public delegate* unmanaged<nint, nint, void> InvokeCallbackCore;
    public delegate* unmanaged<nint, CModelIndex, int> qmodel_rowcount;
    public delegate* unmanaged<nint, CModelIndex, QtItemDataRole, CVariant> qmodel_data;
    public delegate* unmanaged<nint, int, QtOrientation, QtItemDataRole, CVariant> qmodel_headerdata;
    public delegate* unmanaged<nint, CModelIndex, int> qmodel_columncount;
    public delegate* unmanaged<nint, int, int, CModelIndex, CModelIndex> qmodel_index;
    public delegate* unmanaged<nint, CModelIndex, CModelIndex> qmodel_parent;
    public delegate* unmanaged<nint, CModelIndex, QtItemFlag> qmodel_flags;
    public delegate* unmanaged<nint, CModelIndex, CVariant, QtItemDataRole, int> qmodel_setdata;
}


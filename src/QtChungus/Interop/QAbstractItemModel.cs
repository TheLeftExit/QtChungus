using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public abstract unsafe partial class QAbstractItemModel : IDisposable, ICReversePInvokeShimInitializer
{
    private readonly QObject _handle;

    [LibraryImport(Qt.libraryName)]
    internal static partial QObject CreateModelCore(nint managedHandle);
    public static QAbstractItemModel FromHandle(nint managedHandle)
    {
        return (QAbstractItemModel)GCHandle.FromIntPtr(managedHandle).Target!;
    }

    public QObject Handle => _handle;

    protected QAbstractItemModel()
    {
        _handle = CreateModelCore((nint)GCHandle.Alloc(this));
    }

    [UnmanagedCallersOnly]
    private static int qmodel_rowcount(nint managedHandle, CStructPtr<CModelIndex> parent)
    {
        return FromHandle(managedHandle).GetRowCount(parent);
    }
    protected abstract int GetRowCount(CModelIndex parent);

    [UnmanagedCallersOnly]
    private static CStructPtr<CVariant> qmodel_data(nint managedHandle, CStructPtr<CModelIndex> index, QtItemDataRole role)
    {
        return CStructPtr<CVariant>.Alloc(FromHandle(managedHandle).GetData(index, role));
    }
    protected abstract CVariant GetData(CModelIndex index, QtItemDataRole role);

    [UnmanagedCallersOnly]
    private static CStructPtr<CVariant> qmodel_headerdata(nint managedHandle, int section, QtOrientation orientation, QtItemDataRole role)
    {
        return CStructPtr<CVariant>.Alloc(FromHandle(managedHandle).GetHeaderData(section, orientation, role));
    }
    protected abstract CVariant GetHeaderData(int section, QtOrientation orientation, QtItemDataRole role);

    [UnmanagedCallersOnly]
    private static int qmodel_columncount(nint managedHandle, CStructPtr<CModelIndex> parent)
    {
        return FromHandle(managedHandle).GetColumnCount(parent);
    }
    protected abstract int GetColumnCount(CModelIndex parent);


    [UnmanagedCallersOnly]
    private static CStructPtr<CModelIndex> qmodel_index(nint managedHandle, int row, int column, CStructPtr<CModelIndex> parent)
    {
        return CStructPtr<CModelIndex>.Alloc(FromHandle(managedHandle).CreateIndex(row, column, parent));
    }
    protected abstract CModelIndex CreateIndex(int row, int column, CModelIndex parent);

    [UnmanagedCallersOnly]
    private static CStructPtr<CModelIndex> qmodel_parent(nint managedHandle, CStructPtr<CModelIndex> index)
    {
        return CStructPtr<CModelIndex>.Alloc(FromHandle(managedHandle).GetParent(index));
    }
    protected abstract CModelIndex GetParent(CModelIndex index);

    [UnmanagedCallersOnly]
    private static QtItemFlag qmodel_flags(nint managedHandle, CStructPtr<CModelIndex> index)
    {
        return FromHandle(managedHandle).GetFlags(index);
    }
    protected abstract QtItemFlag GetFlags(CModelIndex index);

    [UnmanagedCallersOnly]
    private static int qmodel_setdata(nint managedHandle, CStructPtr<CModelIndex> index, CStructPtr<CVariant> value, QtItemDataRole role)
    {
        return FromHandle(managedHandle).TrySetData(index, value, role) ? 1 : 0;
    }
    protected abstract bool TrySetData(CModelIndex index, CVariant value, QtItemDataRole role);

    [LibraryImport(Qt.libraryName)]
    private static partial void EmitDataChangedCore(QObject model, CStructPtr<CModelIndex> topLeft, CStructPtr<CModelIndex> bottomRight);
    protected void EmitDataChanged(CModelIndex topLeft, CModelIndex bottomRight)
    {
        EmitDataChangedCore(_handle, CStructPtr<CModelIndex>.Alloc(topLeft), CStructPtr<CModelIndex>.Alloc(bottomRight));
    }

    public virtual void Dispose()
    {
        var gcHandle = (GCHandle)_handle.GetProperty<nint>("managedHandle");
        Qt.qobject_delete(_handle);
        gcHandle.Free();
    }

    static void ICReversePInvokeShimInitializer.Initialize(ref CReversePInvokeShim shim)
    {
        shim.qmodel_rowcount = &qmodel_rowcount;
        shim.qmodel_data = &qmodel_data;
        shim.qmodel_headerdata = &qmodel_headerdata;
        shim.qmodel_columncount = &qmodel_columncount;
        shim.qmodel_index = &qmodel_index;
        shim.qmodel_parent = &qmodel_parent;
        shim.qmodel_flags = &qmodel_flags;
        shim.qmodel_setdata = &qmodel_setdata;
    }


}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CModelIndex
{
    public int Row { get; }
    public int Column { get; }
    public nint InternalPointer { get; }
    private int _isValid;

    public bool IsValid
    {
        get => _isValid != 0;
    }

    public CModelIndex(int row, int column, nint internalPointer = 0)
    {
        (Row, Column, InternalPointer) = (row, column, internalPointer);
        _isValid = 1;
    }
}


using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace QtChungus.Internal;

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

public abstract unsafe partial class QcAbstractItemModel : IDisposable
{
    private readonly QObjectStar _handle;

    [LibraryImport(QObject.libraryName)]
    private static partial QObjectStar CreateAbstractItemModel(nint managedHandle, void** vmt);
    public static QcAbstractItemModel FromHandle(nint managedHandle)
    {
        return (QcAbstractItemModel)GCHandle.FromIntPtr(managedHandle).Target!;
    }

    public QObjectStar Handle => _handle;

    protected QcAbstractItemModel()
    {
        var vmt = stackalloc void*[8]
        {
            (delegate* unmanaged<nint, CModelIndex, int>)&qmodel_rowcount,
            (delegate* unmanaged<nint, CModelIndex, QtItemDataRole, CVariant>)&qmodel_data,
            (delegate* unmanaged<nint, int, QtOrientation, QtItemDataRole, CVariant>)&qmodel_headerdata,
            (delegate* unmanaged<nint, CModelIndex, int>)&qmodel_columncount,
            (delegate* unmanaged<nint, int, int, CModelIndex, CModelIndex>)&qmodel_index,
            (delegate* unmanaged<nint, CModelIndex, CModelIndex>)&qmodel_parent,
            (delegate* unmanaged<nint, CModelIndex, QtItemFlag>)&qmodel_flags,
            (delegate* unmanaged<nint, CModelIndex, CVariant, QtItemDataRole, int>)&qmodel_setdata
        };
        _handle = CreateAbstractItemModel((nint)GCHandle.Alloc(this), vmt);
    }

    protected static void DoStuff() { }

    [UnmanagedCallersOnly]
    private static int qmodel_rowcount(nint managedHandle, CModelIndex parent)
    {
        return FromHandle(managedHandle).GetRowCount(parent);
    }
    protected abstract int GetRowCount(CModelIndex parent);

    [UnmanagedCallersOnly]
    private static CVariant qmodel_data(nint managedHandle, CModelIndex index, QtItemDataRole role)
    {
        return FromHandle(managedHandle).GetData(index, role);
    }
    protected abstract CVariant GetData(CModelIndex index, QtItemDataRole role);

    [UnmanagedCallersOnly]
    private static CVariant qmodel_headerdata(nint managedHandle, int section, QtOrientation orientation, QtItemDataRole role)
    {
        return FromHandle(managedHandle).GetHeaderData(section, orientation, role);
    }
    protected abstract CVariant GetHeaderData(int section, QtOrientation orientation, QtItemDataRole role);

    [UnmanagedCallersOnly]
    private static int qmodel_columncount(nint managedHandle, CModelIndex parent)
    {
        return FromHandle(managedHandle).GetColumnCount(parent);
    }
    protected abstract int GetColumnCount(CModelIndex parent);


    [UnmanagedCallersOnly]
    private static CModelIndex qmodel_index(nint managedHandle, int row, int column, CModelIndex parent)
    {
        return FromHandle(managedHandle).CreateIndex(row, column, parent);
    }
    protected abstract CModelIndex CreateIndex(int row, int column, CModelIndex parent);

    [UnmanagedCallersOnly]
    private static CModelIndex qmodel_parent(nint managedHandle, CModelIndex index)
    {
        return FromHandle(managedHandle).GetParent(index);
    }
    protected abstract CModelIndex GetParent(CModelIndex index);

    [UnmanagedCallersOnly]
    private static QtItemFlag qmodel_flags(nint managedHandle, CModelIndex index)
    {
        return FromHandle(managedHandle).GetFlags(index);
    }
    protected abstract QtItemFlag GetFlags(CModelIndex index);

    [UnmanagedCallersOnly]
    private static int qmodel_setdata(nint managedHandle, CModelIndex index, CVariant value, QtItemDataRole role)
    {
        return FromHandle(managedHandle).TrySetData(index, value, role) ? 1 : 0;
    }
    protected abstract bool TrySetData(CModelIndex index, CVariant value, QtItemDataRole role);

    [LibraryImport(QObject.libraryName)]
    private static partial void EmitDataChangedCore(QObjectStar model, CModelIndex topLeft, CModelIndex bottomRight);
    protected void EmitDataChanged(CModelIndex topLeft, CModelIndex bottomRight)
    {
        EmitDataChangedCore(_handle, topLeft, bottomRight);
    }

    public virtual void Dispose()
    {
        var gcHandle = (GCHandle)_handle.GetProperty<nint>("managedHandle");
        Qt.delete(_handle);
        gcHandle.Free();
    }
}


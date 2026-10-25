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
    private readonly QcAbstractItemModel_Unmanaged _unmanagedHandle;
    internal QcAbstractItemModel_Unmanaged UnmanagedModel => _unmanagedHandle;

    protected QcAbstractItemModel()
    {
        var vmt = stackalloc void*[8]
        {
            (delegate* unmanaged<QcAbstractItemModel_Unmanaged, CModelIndex, int>)&rowCount,
            (delegate* unmanaged<QcAbstractItemModel_Unmanaged, CModelIndex, QtItemDataRole, CVariant>)&data,
            (delegate* unmanaged<QcAbstractItemModel_Unmanaged, int, QtOrientation, QtItemDataRole, CVariant>)&headerdata,
            (delegate* unmanaged<QcAbstractItemModel_Unmanaged, CModelIndex, int>)&columncount,
            (delegate* unmanaged<QcAbstractItemModel_Unmanaged, int, int, CModelIndex, CModelIndex>)&index,
            (delegate* unmanaged<QcAbstractItemModel_Unmanaged, CModelIndex, CModelIndex>)&parent,
            (delegate* unmanaged<QcAbstractItemModel_Unmanaged, CModelIndex, QtItemFlag>)&flags,
            (delegate* unmanaged<QcAbstractItemModel_Unmanaged, CModelIndex, CVariant, QtItemDataRole, int>)&setdata
        };
        _unmanagedHandle = new(this, vmt);
    }

    [UnmanagedCallersOnly]
    private static int rowCount(QcAbstractItemModel_Unmanaged sender, CModelIndex parent)
    {
        return sender.ManagedModel.GetRowCount(parent);
    }
    protected abstract int GetRowCount(CModelIndex parent);

    [UnmanagedCallersOnly]
    private static CVariant data(QcAbstractItemModel_Unmanaged sender, CModelIndex index, QtItemDataRole role)
    {
        return sender.ManagedModel.GetData(index, role);
    }
    protected abstract CVariant GetData(CModelIndex index, QtItemDataRole role);

    [UnmanagedCallersOnly]
    private static CVariant headerdata(QcAbstractItemModel_Unmanaged sender, int section, QtOrientation orientation, QtItemDataRole role)
    {
        return sender.ManagedModel.GetHeaderData(section, orientation, role);
    }
    protected abstract CVariant GetHeaderData(int section, QtOrientation orientation, QtItemDataRole role);

    [UnmanagedCallersOnly]
    private static int columncount(QcAbstractItemModel_Unmanaged sender, CModelIndex parent)
    {
        return sender.ManagedModel.GetColumnCount(parent);
    }
    protected abstract int GetColumnCount(CModelIndex parent);


    [UnmanagedCallersOnly]
    private static CModelIndex index(QcAbstractItemModel_Unmanaged sender, int row, int column, CModelIndex parent)
    {
        return sender.ManagedModel.CreateIndex(row, column, parent);
    }
    protected abstract CModelIndex CreateIndex(int row, int column, CModelIndex parent);

    [UnmanagedCallersOnly]
    private static CModelIndex parent(QcAbstractItemModel_Unmanaged sender, CModelIndex index)
    {
        return sender.ManagedModel.GetParent(index);
    }
    protected abstract CModelIndex GetParent(CModelIndex index);

    [UnmanagedCallersOnly]
    private static QtItemFlag flags(QcAbstractItemModel_Unmanaged sender, CModelIndex index)
    {
        return sender.ManagedModel.GetFlags(index);
    }
    protected abstract QtItemFlag GetFlags(CModelIndex index);

    [UnmanagedCallersOnly]
    private static int setdata(QcAbstractItemModel_Unmanaged sender, CModelIndex index, CVariant value, QtItemDataRole role)
    {
        return sender.ManagedModel.TrySetData(index, value, role) ? 1 : 0;
    }
    protected abstract bool TrySetData(CModelIndex index, CVariant value, QtItemDataRole role);

    protected void EmitDataChanged(CModelIndex topLeft, CModelIndex bottomRight)
    {
        _unmanagedHandle.EmitDataChanged(topLeft, bottomRight);
    }

    public virtual void Dispose()
    {
        _unmanagedHandle.Dispose();
    }
}


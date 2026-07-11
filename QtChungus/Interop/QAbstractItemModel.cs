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

    private static T* CopyToHeap<T>(T value) where T : unmanaged
    {
        var pointer = (T*)NativeMemory.Alloc((nuint)sizeof(T));
        *pointer = value;
        return pointer;
    }

    [UnmanagedCallersOnly(EntryPoint = "qmodel_rowcount")]
    private static int qmodel_rowcount(nint managedHandle, CModelIndex* parent)
    {
        return FromHandle(managedHandle).GetRowCount(*parent);
    }
    protected abstract int GetRowCount(CModelIndex parent);

    [UnmanagedCallersOnly(EntryPoint = "qmodel_data")]
    private static QVariant* qmodel_data(nint managedHandle, CModelIndex* index, QtItemDataRole role)
    {
        return CopyToHeap(FromHandle(managedHandle).GetData(*index, role));
    }
    protected abstract QVariant GetData(CModelIndex index, QtItemDataRole role);

    [UnmanagedCallersOnly(EntryPoint = "qmodel_headerdata")]
    private static QVariant* qmodel_headerdata(nint managedHandle, int section, QtOrientation orientation, QtItemDataRole role)
    {
        return CopyToHeap(FromHandle(managedHandle).GetHeaderData(section, orientation, role));
    }
    protected abstract QVariant GetHeaderData(int section, QtOrientation orientation, QtItemDataRole role);

    [UnmanagedCallersOnly(EntryPoint = "qmodel_columncount")]
    private static int qmodel_columncount(nint managedHandle, CModelIndex* parent)
    {
        return FromHandle(managedHandle).GetColumnCount(*parent);
    }
    protected abstract int GetColumnCount(CModelIndex parent);


    [UnmanagedCallersOnly(EntryPoint = "qmodel_index")]
    private static CModelIndex* qmodel_index(nint managedHandle, int row, int column, CModelIndex* parent)
    {
        return CopyToHeap(FromHandle(managedHandle).CreateIndex(row, column, *parent));
    }
    protected abstract CModelIndex CreateIndex(int row, int column, CModelIndex parent);

    [UnmanagedCallersOnly(EntryPoint = "qmodel_parent")]
    private static CModelIndex* qmodel_parent(nint managedHandle, CModelIndex* index)
    {
        return CopyToHeap(FromHandle(managedHandle).GetParent(*index));
    }
    protected abstract CModelIndex GetParent(CModelIndex index);

    [UnmanagedCallersOnly(EntryPoint = "qmodel_flags")]
    private static QtItemFlag qmodel_flags(nint managedHandle, CModelIndex* index)
    {
        return FromHandle(managedHandle).GetFlags(*index);
    }
    protected abstract QtItemFlag GetFlags(CModelIndex index);

    [UnmanagedCallersOnly(EntryPoint = "qmodel_setdata")]
    private static int qmodel_setdata(nint managedHandle, CModelIndex* index, QVariant* value, QtItemDataRole role)
    {
        return FromHandle(managedHandle).TrySetData(*index, *value, role) ? 1 : 0;
    }
    protected abstract bool TrySetData(CModelIndex index, QVariant value, QtItemDataRole role);

    [LibraryImport(Qt.libraryName)]
    private static partial void EmitDataChangedCore(QObject model, CModelIndex* topLeft, CModelIndex* bottomRight);
    protected void EmitDataChanged(CModelIndex topLeft, CModelIndex bottomRight)
    {
        EmitDataChangedCore(_handle, CopyToHeap(topLeft), CopyToHeap(bottomRight));
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

// Enum definitions ripped from F12-ing them in the C++ editor in VS Code

public enum QtItemDataRole : int
{
    DisplayRole = 0,
    DecorationRole = 1,
    EditRole = 2,
    ToolTipRole = 3,
    StatusTipRole = 4,
    WhatsThisRole = 5,
    // Metadata
    FontRole = 6,
    TextAlignmentRole = 7,
    BackgroundRole = 8,
    ForegroundRole = 9,
    CheckStateRole = 10,
    // Accessibility
    AccessibleTextRole = 11,
    AccessibleDescriptionRole = 12,
    // More general purpose
    SizeHintRole = 13,
    InitialSortOrderRole = 14,
    // Internal UiLib roles. Start worrying when public roles go that high.
    DisplayPropertyRole = 27,
    DecorationPropertyRole = 28,
    ToolTipPropertyRole = 29,
    StatusTipPropertyRole = 30,
    WhatsThisPropertyRole = 31,
    // QRangeModel support for QML's required property var modelData
    RangeModelDataRole = 40,
    // QRangeModelAdapter support for accessing entire multi-role objects
    RangeModelAdapterRole = 41,

    // Reserved
    UserRole = 0x0100,

    // Used by Qt models
    StandardItemFlagsRole = UserRole - 1,  // QStandardItemModel
    FileInfoRole = UserRole - 4,           // QFileSystemModel
    RemoteObjectsCacheRole = UserRole - 1, // QtRemoteObjects::QAbstractItemModelReplica
};

public enum QtOrientation : int
{
    Horizontal = 0x1,
    Vertical = 0x2
};

public enum QtItemFlag : int
{
    NoItemFlags = 0,
    ItemIsSelectable = 1,
    ItemIsEditable = 2,
    ItemIsDragEnabled = 4,
    ItemIsDropEnabled = 8,
    ItemIsUserCheckable = 16,
    ItemIsEnabled = 32,
    ItemIsAutoTristate = 64,
    ItemNeverHasChildren = 128,
    ItemIsUserTristate = 256,

    Default = ItemIsSelectable | ItemIsEnabled
};
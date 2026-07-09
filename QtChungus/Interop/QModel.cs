using System.Buffers;
using System.IO.Pipelines;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class QModelColumn<TRow, TValue> : IQModelColumn<TRow>
    where TRow : class
{
    private readonly string _caption;
    private readonly ViewModelGetMethod<TRow, TValue> _getMethod;
    private readonly ViewModelSetMethod<TRow, TValue>? _setMethod;
    private readonly QVariant.TypeCode _type;

    public QModelColumn(Expression<Func<TRow, TValue>> accessor, string? caption = null)
    {
        _type = QVariant.GetTypeCodeOrThrow<TValue>();
        if (!ExpressionHelper.TryParseProperty(accessor, out var propertyInfo))
        {
            throw new NotSupportedException();
        }
        _caption = caption ?? propertyInfo.Name;
        _getMethod = propertyInfo.GetMethod!.CreateDelegate<ViewModelGetMethod<TRow, TValue>>();
        _setMethod = propertyInfo.SetMethod?.CreateDelegate<ViewModelSetMethod<TRow, TValue>>();
    }

    QVariant IQModelColumn<TRow>.Caption => new(_caption);
    QVariant IQModelColumn<TRow>.GetValue(TRow owner) => QVariant.Create(_getMethod(owner));
    bool IQModelColumn<TRow>.Editable => _setMethod is not null;
    void IQModelColumn<TRow>.SetValue(TRow owner, QVariant value) => _setMethod!(owner, value.Get<TValue>());
    QVariant.TypeCode IQModelColumn<TRow>.Type => _type;
}

public interface IQModelColumn<T> where T : class
{
    QVariant Caption { get; }
    QVariant GetValue(T owner);
    bool Editable { get; }
    void SetValue(T owner, QVariant value);
    QVariant.TypeCode Type { get; }
}

public partial class QListModel<T> : IQModel where T : class
{
    private readonly QObject _pointer;
    private readonly IList<T> _list;
    private readonly IQModelColumn<T>[] _columns;

    public QListModel(IList<T> list, IQModelColumn<T>[] columns)
    {
        _pointer = QModel.CreateModelCore((nint)GCHandle.Alloc(this));
        _list = list;
        _columns = columns;
    }

    public QObject GetHandle() => _pointer;

    CModelIndex IQModel.CreateIndex(int row, int column, CModelIndex parent)
    {
        return new CModelIndex(row, column);
    }

    int IQModel.GetColumnCount(CModelIndex parent)
    {
        if(parent.IsValid) return 0;
        return _columns.Length;
    }

    QVariant IQModel.GetData(CModelIndex index, QtItemDataRole role)
    {
        if(role is not QtItemDataRole.DisplayRole and not QtItemDataRole.EditRole) return new();
        return _columns[index.Column].GetValue(_list[index.Row]);
    }

    QtItemFlag IQModel.GetFlags(CModelIndex index)
    {
        var result = QtItemFlag.Default;
        if (_columns[index.Column].Editable)
        {
            result |= QtItemFlag.ItemIsEditable;
        }
        return result;
    }

    QVariant IQModel.GetHeaderData(int section, QtOrientation orientation, QtItemDataRole role)
    {
        if(role is not QtItemDataRole.DisplayRole) return new();
        return _columns[section].Caption;
    }

    CModelIndex IQModel.GetParent(CModelIndex index)
    {
        return new();
    }

    int IQModel.GetRowCount(CModelIndex parent)
    {
        if (parent.IsValid)
        {
            return 0;
        }
        return _list.Count;
    }

    bool IQModel.TrySetData(CModelIndex index, QVariant value, QtItemDataRole role)
    {
        var column = _columns[index.Column];
        if (!column.Editable)
        {
            return false;
        }
        column.SetValue(_list[index.Row], value);
        QModel.EmitDataChanged(_pointer, index, index);
        return true;
    }

    public void Dispose()
    {
        var gcHandle = (GCHandle)_pointer.GetProperty<nint>("managedHandle");
        Qt.qobject_delete(_pointer);
        gcHandle.Free();
    }
}

public interface IQModel : IDisposable
{
    int GetRowCount(CModelIndex parent);
    QVariant GetData(CModelIndex index, QtItemDataRole role);
    QVariant GetHeaderData(int section, QtOrientation orientation, QtItemDataRole role);
    int GetColumnCount(CModelIndex parent);
    CModelIndex CreateIndex(int row, int column, CModelIndex parent);
    CModelIndex GetParent(CModelIndex index);
    QtItemFlag GetFlags(CModelIndex index);
    bool TrySetData(CModelIndex index, QVariant value, QtItemDataRole role);
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct CModelIndex
{
    public int Row {get;}
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

public static unsafe partial class QModel
{
    [LibraryImport(Qt.libraryName)]
    internal static partial QObject CreateModelCore(nint managedHandle);

    [LibraryImport(Qt.libraryName)]
    private static partial void EmitDataChangedCore(QObject model, CModelIndex* topLeft, CModelIndex* bottomRight);
    internal static void EmitDataChanged(QObject model, CModelIndex topLeft, CModelIndex bottomRight)
    {
        EmitDataChangedCore(model, CopyToHeap(topLeft), CopyToHeap(bottomRight));
    }

    public static IQModel FromHandle(nint managedHandle, [CallerMemberName] string? caller = null)
    {
        //Console.WriteLine(caller);
        return (IQModel)GCHandle.FromIntPtr(managedHandle).Target!;
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
    [UnmanagedCallersOnly(EntryPoint = "qmodel_data")]
    private static QVariant* qmodel_data(nint managedHandle, CModelIndex* index, int role)
    {
        return CopyToHeap(FromHandle(managedHandle).GetData(*index, (QtItemDataRole)role));
    }
    [UnmanagedCallersOnly(EntryPoint = "qmodel_headerdata")]
    private static QVariant* qmodel_headerdata(nint managedHandle, int section, int orientation, int role)
    {
        return CopyToHeap(FromHandle(managedHandle).GetHeaderData(section, (QtOrientation)orientation, (QtItemDataRole)role));
    }
    [UnmanagedCallersOnly(EntryPoint = "qmodel_columncount")]
    private static int qmodel_columncount(nint managedHandle, CModelIndex* parent)
    {
        return FromHandle(managedHandle).GetColumnCount(*parent);
    }
    [UnmanagedCallersOnly(EntryPoint = "qmodel_index")]
    private static CModelIndex* qmodel_index(nint managedHandle, int row, int column, CModelIndex* parent)
    {
        return CopyToHeap(FromHandle(managedHandle).CreateIndex(row, column, *parent));
    }
    [UnmanagedCallersOnly(EntryPoint = "qmodel_parent")]
    private static CModelIndex* qmodel_parent(nint managedHandle, CModelIndex* index)
    {
        return CopyToHeap(FromHandle(managedHandle).GetParent(*index));
    }
    [UnmanagedCallersOnly(EntryPoint = "qmodel_flags")]
    private static int qmodel_flags(nint managedHandle, CModelIndex* index)
    {
        return (int)FromHandle(managedHandle).GetFlags(*index);
    }
    [UnmanagedCallersOnly(EntryPoint = "qmodel_setdata")]
    private static int qmodel_setdata(nint managedHandle, CModelIndex* index, QVariant* value, int role)
    {
        return FromHandle(managedHandle).TrySetData(*index, *value, (QtItemDataRole)role) ? 1 : 0;
    }
}

public enum QtItemDataRole
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

public enum QtOrientation
{
    Horizontal = 0x1,
    Vertical = 0x2
};

public enum QtItemFlag
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
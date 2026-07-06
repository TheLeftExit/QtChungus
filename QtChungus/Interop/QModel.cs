using System.Buffers;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class QModelColumn<TRow, TValue> : IQModelColumn<TRow>
    where TRow : class
{
    private readonly string _caption;
    private readonly ViewModelGetMethod<TRow, TValue> _getMethod;
    private readonly ViewModelSetMethod<TRow, TValue>? _setMethod;

    public QModelColumn(Expression<Func<TRow, TValue>> accessor, string? caption = null)
    {
        QVariant.EnsureSupportedType<TValue>();
        if(!ExpressionHelper.TryParseProperty(accessor, out var propertyInfo))
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
    void IQModelColumn<TRow>.SetValue(TRow owner, QVariant value) =>_setMethod!(owner, value.Get<TValue>());
}

public interface IQModelColumn<T> where T : class
{
    QVariant Caption { get; }
    QVariant GetValue(T owner);
    bool Editable { get; }
    void SetValue(T owner, QVariant value);
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

    public int RowCount => _list.Count;
    public int ColumnCount => _columns.Length;
    public QVariant Data(int row, int column) => _columns[column].GetValue(_list[row]);
    public QVariant HeaderData(int column) => _columns[column].Caption;
    public bool IsItemEditable(int row, int column) => _columns[column].Editable;
    public QObject GetHandle() => _pointer;
    public void SetData(int row, int column, QVariant value) => _columns[column].SetValue(_list[row], value);
    public void Dispose()
    {
        var gcHandle = (GCHandle)_pointer.GetProperty<nint>("managedHandle");
        Qt.qobject_delete(_pointer);
        gcHandle.Free();
    }
}

public interface IQModel : IDisposable
{
    int RowCount { get; }
    QVariant Data(int row, int column);
    QVariant HeaderData(int column);
    int ColumnCount { get; }
    bool IsItemEditable(int row, int column);
    void SetData(int row, int column, QVariant value);
}


public static unsafe partial class QModel
{
    [LibraryImport(Qt.libraryName)]
    internal static partial QObject CreateModelCore(nint managedHandle);

    public static IQModel FromHandle(nint managedHandle) => (IQModel)GCHandle.FromIntPtr(managedHandle).Target!;

    [UnmanagedCallersOnly(EntryPoint = "qmodel_rowcount")]
    private static int qmodel_rowcount(nint managedHandle)
    {
        return FromHandle(managedHandle).RowCount;
    }

    [UnmanagedCallersOnly(EntryPoint = "qmodel_data")]
    private static QVariant qmodel_data(nint managedHandle, int row, int column)
    {
        return FromHandle(managedHandle).Data(row, column);
    }

    [UnmanagedCallersOnly(EntryPoint = "qmodel_headerdata")]
    private static QVariant qmodel_headerdata(nint managedHandle, int column)
    {
        return FromHandle(managedHandle).HeaderData(column);
    }

    [UnmanagedCallersOnly(EntryPoint = "qmodel_columncount")]
    private static int qmodel_columncount(nint managedHandle)
    {
        return FromHandle(managedHandle).ColumnCount;
    }

    [UnmanagedCallersOnly(EntryPoint = "qmodel_isitemeditable")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static bool qmodel_isitemeditable(nint managedHandle, int row, int column)
    {
        return FromHandle(managedHandle).IsItemEditable(row, column);
    }

    [UnmanagedCallersOnly(EntryPoint = "qmodel_setdata")]
    private static void qmodel_setdata(nint managedHandle, int row, int column, QVariant value)
    {
        FromHandle(managedHandle).SetData(row, column, value);
        value.Dispose();
    }
}
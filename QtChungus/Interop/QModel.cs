using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class QModelColumn<TRow, TValue> : IQModelColumn<TRow> where TValue : notnull
{
    private readonly string _caption;
    private readonly Func<TRow, TValue> _accessor;

    public QModelColumn(string caption, Func<TRow, TValue> accessor)
    {
        QVariant.EnsureSupportedType<TValue>();
        (_caption, _accessor) = (caption, accessor);
    }

    QVariant IQModelColumn<TRow>.Caption => new(_caption);
    QVariant IQModelColumn<TRow>.GetValue(TRow owner) => QVariant.Create(_accessor(owner));
}

public interface IQModelColumn<T>
{
    QVariant Caption { get; }
    QVariant GetValue(T owner);
}

public partial class QListModel<T> : IQModel
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
    public QObject GetHandle() => _pointer;
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
    private static void qmodel_data(nint managedHandle, int row, int column, QVariant.TypeCode* type, void** value)
    {
        FromHandle(managedHandle).Data(row, column).Write(type, value);
    }

    [UnmanagedCallersOnly(EntryPoint = "qmodel_headerdata")]
    private static void qmodel_headerdata(nint managedHandle, int column, QVariant.TypeCode* type, void** value)
    {
        FromHandle(managedHandle).HeaderData(column).Write(type, value);
    }

    [UnmanagedCallersOnly(EntryPoint = "qmodel_columncount")]
    private static int qmodel_columncount(nint managedHandle)
    {
        return FromHandle(managedHandle).ColumnCount;
    }
}
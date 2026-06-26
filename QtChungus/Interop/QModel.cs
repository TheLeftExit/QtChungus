using System.Buffers;
using System.Runtime.InteropServices;

public interface IQModelColumn<T>
{
    string Caption { get; }
    QVariant GetValue(T owner);
}

public partial class QModel<T>
{
    private readonly QObject _pointer;
    private readonly IList<T> _list;
    private readonly IQModelColumn<T>[] _columns;

    public QModel(IList<T> list, IQModelColumn<T>[] columns)
    {
        _pointer = QModelGlue.CreateModel((nint)GCHandle.Alloc(this));
        _list = list;
        _columns = columns;
    }

    public int RowCount => _list.Count;
}

public interface IQModel
{
    int RowCount { get; }
}


public partial class QModelGlue
{
    [LibraryImport(Qt.libraryName)]
    public static partial QObject CreateModel(nint managedHandle);

    private static IQModel GetModel(nint managedHandle) => (IQModel)GCHandle.FromIntPtr(managedHandle).Target!;

    [UnmanagedCallersOnly(EntryPoint = "qmodel_rowcount")]
    private static int qmodel_rowcount(nint managedHandle) => GetModel(managedHandle).RowCount;
}
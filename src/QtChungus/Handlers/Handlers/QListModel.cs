using System.Linq.Expressions;

namespace QtChungus.Internal;

public class QModelColumn<TRow, TValue> : IQModelColumn<TRow>
    where TRow : class
{
    private readonly string _caption;
    private readonly ViewModelGetMethod<TRow, TValue> _getMethod;
    private readonly ViewModelSetMethod<TRow, TValue>? _setMethod;
    private readonly CVariant.TypeCode _type;

    public QModelColumn(Expression<Func<TRow, TValue>> accessor, string? caption = null)
    {
        _type = CVariant.GetTypeCodeOrThrow<TValue>();
        if (!ExpressionHelper.TryParseProperty(accessor, out var propertyInfo))
        {
            throw new NotSupportedException();
        }
        _caption = caption ?? propertyInfo.Name;
        _getMethod = propertyInfo.GetMethod!.CreateDelegate<ViewModelGetMethod<TRow, TValue>>();
        _setMethod = propertyInfo.SetMethod?.CreateDelegate<ViewModelSetMethod<TRow, TValue>>();
    }

    CVariant IQModelColumn<TRow>.Caption => _caption;
    CVariant IQModelColumn<TRow>.GetValue(TRow owner) => CVariant.Create(_getMethod(owner));
    bool IQModelColumn<TRow>.Editable => _setMethod is not null;
    void IQModelColumn<TRow>.SetValue(TRow owner, CVariant value) => _setMethod!(owner, value.Get<TValue>());
    CVariant.TypeCode IQModelColumn<TRow>.Type => _type;
}

public interface IQModelColumn<T> where T : class
{
    CVariant Caption { get; }
    CVariant GetValue(T owner);
    bool Editable { get; }
    void SetValue(T owner, CVariant value);
    CVariant.TypeCode Type { get; }
}

public partial class QListModel<T> : QAbstractItemModel where T : class
{
    private readonly IList<T> _list;
    private readonly IQModelColumn<T>[] _columns;

    public QListModel(IList<T> list, IQModelColumn<T>[] columns)
    {
        _list = list;
        _columns = columns;
    }

    protected override CModelIndex CreateIndex(int row, int column, CModelIndex parent)
    {
        return new CModelIndex(row, column);
    }

    protected override int GetColumnCount(CModelIndex parent)
    {
        if (parent.IsValid) return 0;
        return _columns.Length;
    }

    protected override CVariant GetData(CModelIndex index, QtItemDataRole role)
    {
        if (role is not QtItemDataRole.DisplayRole and not QtItemDataRole.EditRole) return new();
        return _columns[index.Column].GetValue(_list[index.Row]);
    }

    protected override QtItemFlag GetFlags(CModelIndex index)
    {
        var result = QtItemFlag.Default;
        if (_columns[index.Column].Editable)
        {
            result |= QtItemFlag.ItemIsEditable;
        }
        return result;
    }

    protected override CVariant GetHeaderData(int section, QtOrientation orientation, QtItemDataRole role)
    {
        if (role is not QtItemDataRole.DisplayRole) return new();
        return _columns[section].Caption;
    }

    protected override CModelIndex GetParent(CModelIndex index)
    {
        return new();
    }

    protected override int GetRowCount(CModelIndex parent)
    {
        if (parent.IsValid)
        {
            return 0;
        }
        return _list.Count;
    }

    protected override bool TrySetData(CModelIndex index, CVariant value, QtItemDataRole role)
    {
        var column = _columns[index.Column];
        if (!column.Editable)
        {
            return false;
        }
        column.SetValue(_list[index.Row], value);
        EmitDataChanged(index, index);
        return true;
    }
}

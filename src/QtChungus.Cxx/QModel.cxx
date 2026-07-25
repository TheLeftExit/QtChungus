#include <QAbstractItemModel>
#include <QMetaType>

#include "CVariant.cxx"
#include "CModelIndex.cxx"

typedef void *GCHandle;

class QModel : public QAbstractItemModel
{
    friend CModelIndex;
    Q_OBJECT
    Q_PROPERTY(void *managedHandle READ managedHandle)
private:
    GCHandle m_managedHandle;

public:
    QModel(void *handle)
    {
        m_managedHandle = handle;
    }
    void *managedHandle()
    {
        return m_managedHandle;
    }
    int rowCount(const QModelIndex &parent = QModelIndex()) const override;
    QVariant data(const QModelIndex &index, int role) const override;
    QVariant headerData(int section, Qt::Orientation orientation, int role = Qt::DisplayRole) const override;
    int columnCount(const QModelIndex &parent) const override;
    QModelIndex index(int row, int column, const QModelIndex &parent = QModelIndex()) const override;
    QModelIndex parent(const QModelIndex &index) const override;
    Qt::ItemFlags flags(const QModelIndex &index) const override;
    bool setData(const QModelIndex &index, const QVariant &value, int role = Qt::EditRole) override;

    QModelIndex decodeIndex(CModelIndex index)
    {
        if (index.IsValid)
        {
            return createIndex(index.Row, index.Column, index.InternalPointer);
        }
        return QModelIndex();
    }
};

extern "C" void *CreateModelCore(void *managedHandle)
{
    return new QModel(managedHandle);
}
extern "C" void EmitDataChangedCore(QModel *model, CModelIndex topLeft, CModelIndex bottomRight)
{
    model->dataChanged(model->decodeIndex(topLeft), model->decodeIndex(bottomRight));
}

#include "QModel.moc"

extern "C" qint32 qmodel_rowcount(GCHandle managedHandle, CModelIndex parent);
int QModel::rowCount(const QModelIndex &parent) const
{
    return qmodel_rowcount(m_managedHandle, CModelIndex(parent));
}

extern "C" CVariant qmodel_data(GCHandle managedHandle, CModelIndex index, qint32 role);
QVariant QModel::data(const QModelIndex &index, int role) const
{
    return qmodel_data(m_managedHandle, index, role);
}

extern "C" CVariant qmodel_headerdata(GCHandle managedHandle, qint32 section, qint32 orientation, qint32 role);
QVariant QModel::headerData(int section, Qt::Orientation orientation, int role) const
{
    return qmodel_headerdata(m_managedHandle, section, orientation, role);
}

extern "C" qint32 qmodel_columncount(GCHandle managedHandle, CModelIndex parent);
int QModel::columnCount(const QModelIndex &parent) const
{
    return qmodel_columncount(m_managedHandle, parent);
}

extern "C" CModelIndex qmodel_index(GCHandle managedHandle, qint32 row, qint32 column, CModelIndex parent);
QModelIndex QModel::index(int row, int column, const QModelIndex &parent) const
{
    CModelIndex result = qmodel_index(m_managedHandle, row, column, parent);
    return result.IsValid ? createIndex(result.Row, result.Column, result.InternalPointer) : QModelIndex();
}

extern "C" CModelIndex qmodel_parent(GCHandle managedHandle, CModelIndex index);
QModelIndex QModel::parent(const QModelIndex &index) const
{
    CModelIndex result = qmodel_parent(m_managedHandle, index);
    return result.IsValid ? createIndex(result.Row, result.Column, result.InternalPointer) : QModelIndex();
}

extern "C" qint32 qmodel_flags(GCHandle managedHandle, CModelIndex index);
Qt::ItemFlags QModel::flags(const QModelIndex &index) const
{
    return static_cast<Qt::ItemFlag>(qmodel_flags(m_managedHandle, index));
}

extern "C" qint32 qmodel_setdata(GCHandle managedHandle, CModelIndex index, CVariant value, qint32 role);
bool QModel::setData(const QModelIndex &index, const QVariant &value, int role)
{
    return qmodel_setdata(m_managedHandle, index, value, role);
}
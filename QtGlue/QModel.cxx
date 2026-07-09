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
        if (*index.isValid())
        {
            return createIndex(*index.row(), *index.column(), *index.internalPointer());
        }
        return QModelIndex();
    }
};

extern "C" void *CreateModelCore(void *managedHandle)
{
    return new QModel(managedHandle);
}
extern "C" void EmitDataChangedCore(QModel *model, CModelIndexPtr topLeft, CModelIndexPtr bottomRight)
{
    model->dataChanged(model->decodeIndex(topLeft), model->decodeIndex(bottomRight));
}

#include "QModel.moc"

#include <iostream>
using namespace std;

extern "C" qint32 qmodel_rowcount(GCHandle managedHandle, CModelIndexPtr parent);
int QModel::rowCount(const QModelIndex &parent) const
{
    return qmodel_rowcount(m_managedHandle, CModelIndex(parent));
}

extern "C" CVariantPtr qmodel_data(GCHandle managedHandle, CModelIndexPtr index, qint32 role);
QVariant QModel::data(const QModelIndex &index, int role) const
{
    return CVariant(qmodel_data(m_managedHandle, CModelIndex(index), role));
}

extern "C" CVariantPtr qmodel_headerdata(GCHandle managedHandle, qint32 section, qint32 orientation, qint32 role);
QVariant QModel::headerData(int section, Qt::Orientation orientation, int role) const
{
    return CVariant(qmodel_headerdata(m_managedHandle, section, orientation, role));
}

extern "C" qint32 qmodel_columncount(GCHandle managedHandle, CModelIndexPtr parent);
int QModel::columnCount(const QModelIndex &parent) const
{
    return qmodel_columncount(m_managedHandle, CModelIndex(parent));
}

extern "C" CModelIndexPtr qmodel_index(GCHandle managedHandle, qint32 row, qint32 column, CModelIndexPtr parent);
QModelIndex QModel::index(int row, int column, const QModelIndex &parent) const
{
    CModelIndex result = qmodel_index(m_managedHandle, row, column, CModelIndex(parent));
    return *result.isValid() ? createIndex(*result.row(), *result.column(), *result.internalPointer()) : QModelIndex();
}

extern "C" CModelIndexPtr qmodel_parent(GCHandle managedHandle, CModelIndexPtr index);
QModelIndex QModel::parent(const QModelIndex &index) const
{
    CModelIndex result = qmodel_parent(m_managedHandle, CModelIndex(index));
    return *result.isValid() ? createIndex(*result.row(), *result.column(), *result.internalPointer()) : QModelIndex();
}

extern "C" qint32 qmodel_flags(GCHandle managedHandle, CModelIndexPtr index);
Qt::ItemFlags QModel::flags(const QModelIndex &index) const
{
    return static_cast<Qt::ItemFlag>(qmodel_flags(m_managedHandle, CModelIndex(index)));
}

extern "C" qint32 qmodel_setdata(GCHandle managedHandle, CModelIndexPtr index, CVariantPtr value, qint32 role);
bool QModel::setData(const QModelIndex &index, const QVariant &value, int role)
{
    return qmodel_setdata(m_managedHandle, CModelIndex(index), CVariant(value), role);
}
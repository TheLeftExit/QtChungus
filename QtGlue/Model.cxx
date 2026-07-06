#include <QAbstractItemModel>
#include <QMetaType>

#include "Variant.h"

extern "C"
{
    int qmodel_rowcount(void *managedHandle);
    QManagedVariant qmodel_data(void *managedHandle, int row, int column);
    QManagedVariant qmodel_headerdata(void *managedHandle, int column);
    int qmodel_columncount(void *managedHandle);
    int qmodel_isitemeditable(void *managedHandle, int row, int column);
    void qmodel_setdata(void *managedHandle, int row, int column, QManagedVariant value);
}

class QModel : public QAbstractItemModel
{
    Q_OBJECT
    Q_PROPERTY(void *managedHandle READ managedHandle)
private:
    void *m_managedHandle;

public:
    void *managedHandle()
    {
        return m_managedHandle;
    }

    QModel(void *handle)
    {
        m_managedHandle = handle;
    }
    int rowCount(const QModelIndex &parent = QModelIndex()) const override
    {
        if (parent.isValid())
        {
            return 0;
        }

        return qmodel_rowcount(m_managedHandle);
    }
    QVariant data(const QModelIndex &index, int role) const override
    {
        QMetaType::Type type;
        void *value;
        QManagedVariant managedResult = qmodel_data(m_managedHandle, index.row(), index.column());
        QVariant result = DecodeQVariant(managedResult);
        if (role == Qt::DisplayRole || role == Qt::EditRole)
        {
            return result;
        }
        return QVariant();
    }
    QVariant headerData(int section, Qt::Orientation orientation, int role = Qt::DisplayRole) const override
    {
        if (orientation != Qt::Horizontal || role != Qt::DisplayRole)
        {
            return QVariant();
        }
        QMetaType::Type type;
        void *value;
        QManagedVariant managedResult = qmodel_headerdata(m_managedHandle, section);
        return DecodeQVariant(managedResult);
    }
    int columnCount(const QModelIndex &parent) const override
    {
        return qmodel_columncount(m_managedHandle);
    }
    QModelIndex index(int row, int column, const QModelIndex &parent = QModelIndex()) const override
    {
        return createIndex(row, column);
    }
    QModelIndex parent(const QModelIndex &index) const override
    {
        return QModelIndex();
    }

    Qt::ItemFlags flags(const QModelIndex &index) const override
    {
        Qt::ItemFlags result = QAbstractItemModel::flags(index);
        if (qmodel_isitemeditable(m_managedHandle, index.row(), index.column()))
        {
            result |= Qt::ItemIsEditable;
        }
        return result;
    }
    bool setData(const QModelIndex &index, const QVariant &value, int role = Qt::EditRole) override
    {
        if (role != Qt::EditRole || !index.isValid())
        {
            return false;
        }
        QManagedVariant managedVariant = EncodeQVariant(value);
        qmodel_setdata(m_managedHandle, index.row(), index.column(), managedVariant);
        emit dataChanged(index, index, {role});
        return true;
    }
};

#include "Model.moc"

extern "C" void *CreateModelCore(void *managedHandle)
{
    return new QModel(managedHandle);
}
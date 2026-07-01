#include <QAbstractItemModel>
#include <QMetaType>

#include "Variant.h"

extern "C"
{
    int qmodel_rowcount(void *managedHandle);
    void qmodel_data(void *managedHandle, int row, int column, QMetaType::Type *type, void **value);
    void qmodel_headerdata(void *managedHandle, int column, QMetaType::Type *type, void **value);
    int qmodel_columncount(void *managedHandle);
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
        if (role != Qt::DisplayRole)
        {
            return QVariant();
        }
        QMetaType::Type type;
        void *value;
        qmodel_data(m_managedHandle, index.row(), index.column(), &type, &value);
        return DecodeQVariant(type, value);
    }
    QVariant headerData(int section, Qt::Orientation orientation, int role = Qt::DisplayRole) const override
    {
        if (orientation != Qt::Horizontal || role != Qt::DisplayRole)
        {
            return QVariant();
        }
        QMetaType::Type type;
        void *value;
        qmodel_headerdata(m_managedHandle, section, &type, &value);
        return DecodeQVariant(type, value);
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
};

#include "Model.moc"

extern "C" void *CreateModelCore(void *managedHandle)
{
    return new QModel(managedHandle);
}
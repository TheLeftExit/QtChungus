#include <QAbstractItemModel>
#include <QMetaType>

#include "CVariant.cxx"
#include "CModelIndex.cxx"

#ifndef QCABSTRACTITEMMODEL_H
#define QCABSTRACTITEMMODEL_H

typedef void *GCHandle;

struct QcAbstractItemModelVMT
{
    qint32 (*rowCount)(GCHandle, CModelIndex);
    CVariant (*data)(GCHandle, CModelIndex, qint32);
    CVariant (*headerData)(GCHandle, qint32, qint32, qint32);
    qint32 (*columnCount)(GCHandle, CModelIndex);
    CModelIndex (*index)(GCHandle, qint32, qint32, CModelIndex);
    CModelIndex (*parent)(GCHandle, CModelIndex);
    qint32 (*flags)(GCHandle, CModelIndex);
    qint32 (*setData)(GCHandle, CModelIndex, CVariant, qint32);
};

class QcAbstractItemModel : public QAbstractItemModel
{
    Q_OBJECT
    Q_PROPERTY(void *managedHandle READ managedHandle)
private:
    GCHandle m_managedHandle;
    QcAbstractItemModelVMT m_vmt;

public:
    QcAbstractItemModel(void *handle, void *vmt)
    {
        m_managedHandle = handle;
        m_vmt = *static_cast<QcAbstractItemModelVMT *>(vmt);
    }
    void emitDataChanged(CModelIndex topLeft, CModelIndex bottomRight)
    {
        dataChanged(createIndexPublic(topLeft), createIndexPublic(bottomRight));
    }
    void *managedHandle()
    {
        return m_managedHandle;
    }
    QModelIndex createIndexPublic(CModelIndex index) const
    {
        if (index.IsValid)
        {
            return createIndex(index.Row, index.Column, index.InternalPointer);
        }
        return QModelIndex();
    }

    int rowCount(const QModelIndex &parent = QModelIndex()) const override
    {
        return m_vmt.rowCount(m_managedHandle, parent);
    }
    QVariant data(const QModelIndex &index, int role) const override
    {
        return m_vmt.data(m_managedHandle, index, role);
    }
    QVariant headerData(int section, Qt::Orientation orientation, int role = Qt::DisplayRole) const override
    {
        return m_vmt.headerData(m_managedHandle, section, orientation, role);
    }
    int columnCount(const QModelIndex &parent) const override
    {
        return m_vmt.columnCount(m_managedHandle, parent);
    }
    QModelIndex index(int row, int column, const QModelIndex &parent = QModelIndex()) const override
    {
        return createIndexPublic(m_vmt.index(m_managedHandle, row, column, parent));
    }
    QModelIndex parent(const QModelIndex &index) const override
    {
        return createIndexPublic(m_vmt.parent(m_managedHandle, index));
    }
    Qt::ItemFlags flags(const QModelIndex &index) const override
    {
        return static_cast<Qt::ItemFlag>(m_vmt.flags(m_managedHandle, index));
    }
    bool setData(const QModelIndex &index, const QVariant &value, int role = Qt::EditRole) override
    {
        return m_vmt.setData(m_managedHandle, index, value, role);
    }
};

#endif
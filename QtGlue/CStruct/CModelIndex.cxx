#include "Base.cxx"
#include <QModelIndex>

#ifndef CMODELINDEX_H
#define CMODELINDEX_H

typedef void *CModelIndexPtr;

struct CModelIndex : public CStruct<CModelIndexPtr>
{
    CModelIndex(CModelIndexPtr pointer) : CStruct(pointer) {}
    CModelIndex(const QModelIndex &index) : CStruct(sizeof(qint32) + sizeof(qint32) + sizeof(void *) + sizeof(qint32))
    {
        if (index.isValid())
        {
            *row() = index.row();
            *column() = index.column();
            *internalPointer() = index.internalPointer();
            *isValid() = 1;
        }
        else
        {
            *isValid() = 0;
        }
    }

    qint32 *row() const
    {
        return Get<qint32>(0);
    }

    qint32 *column() const
    {
        return Get<qint32>(sizeof(qint32));
    }

    void **internalPointer() const
    {
        return Get<void *>(sizeof(qint32) + sizeof(qint32));
    }

    qint32 *isValid() const
    {
        return Get<qint32>(sizeof(qint32) + sizeof(qint32) + sizeof(void *));
    }
};

#endif
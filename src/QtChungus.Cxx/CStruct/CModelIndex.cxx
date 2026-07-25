#include <QModelIndex>

#ifndef CMODELINDEX_H
#define CMODELINDEX_H

struct CModelIndex
{
    qint32 Row;
    qint32 Column;
    void *InternalPointer;
    qint32 IsValid;

    CModelIndex(QModelIndex index)
    {
        if (index.isValid())
        {
            Row = index.row();
            Column = index.column();
            InternalPointer = index.internalPointer();
            IsValid = 1;
        }
        else
        {
            IsValid = 0;
        }
    }
};

#endif
#include <QAbstractItemModel>

class QModel : public QAbstractItemModel
{
    Q_OBJECT
private:
    void *managedHandle;

public:
    QModel(void *handle)
    {
        managedHandle = handle;
    }
    int rowCount(const QModelIndex &parent = QModelIndex()) const override
    {
        
    }
    QVariant data(const QModelIndex &index, int role) const override
    {
    }
    QVariant headerData(int section, Qt::Orientation orientation, int role = Qt::DisplayRole) const override
    {

    }
    int columnCount(const QModelIndex &parent) const override {

    }
    QModelIndex index(int row, int column, const QModelIndex &parent = QModelIndex()) const override{

    }
    QModelIndex parent(const QModelIndex &index) const override{
        
    }
};

#include "Model.moc"

extern "C" void *CreateModel(void* managedHandle){
    return new QModel(managedHandle);
}
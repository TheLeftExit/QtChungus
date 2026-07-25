#include <QtGlobal>
#include "CVariant.cxx"
#include "CModelIndex.cxx"

// using GCHandle = void *;
typedef void *GCHandle;

struct CReversePInvokeShim
{
    void (*RouteSignalCore)(QObject *, const char *);
    void (*InvokeCallbackCore)(void *, void *);
    qint32 (*qmodel_rowcount)(GCHandle, CModelIndex);
    CVariant (*qmodel_data)(GCHandle, CModelIndex, qint32);
    CVariant (*qmodel_headerdata)(GCHandle, qint32, qint32, qint32);
    qint32 (*qmodel_columncount)(GCHandle, CModelIndex);
    CModelIndex (*qmodel_index)(GCHandle, qint32, qint32, CModelIndex);
    CModelIndex (*qmodel_parent)(GCHandle, CModelIndex);
    qint32 (*qmodel_flags)(GCHandle, CModelIndex);
    qint32 (*qmodel_setdata)(GCHandle, CModelIndex, CVariant, qint32);
};

CReversePInvokeShim *shim;

extern "C" void InitializeShim(CReversePInvokeShim *pointer)
{
    shim = pointer;
}

extern "C" void RouteSignalCore(QObject *sender, const char *signalName) { shim->RouteSignalCore(sender, signalName); }
extern "C" void InvokeCallbackCore(void *callbackHandle, void *stateHandle) { shim->InvokeCallbackCore(callbackHandle, stateHandle); }
extern "C" qint32 qmodel_rowcount(GCHandle managedHandle, CModelIndex parent) { return shim->qmodel_rowcount(managedHandle, parent); }
extern "C" CVariant qmodel_data(GCHandle managedHandle, CModelIndex index, qint32 role) { return shim->qmodel_data(managedHandle, index, role); }
extern "C" CVariant qmodel_headerdata(GCHandle managedHandle, qint32 section, qint32 orientation, qint32 role) { return shim->qmodel_headerdata(managedHandle, section, orientation, role); }
extern "C" qint32 qmodel_columncount(GCHandle managedHandle, CModelIndex parent) { return shim->qmodel_columncount(managedHandle, parent); }
extern "C" CModelIndex qmodel_index(GCHandle managedHandle, qint32 row, qint32 column, CModelIndex parent) { return shim->qmodel_index(managedHandle, row, column, parent); }
extern "C" CModelIndex qmodel_parent(GCHandle managedHandle, CModelIndex index) { return shim->qmodel_parent(managedHandle, index); }
extern "C" qint32 qmodel_flags(GCHandle managedHandle, CModelIndex index) { return shim->qmodel_flags(managedHandle, index); }
extern "C" qint32 qmodel_setdata(GCHandle managedHandle, CModelIndex index, CVariant value, qint32 role) { return shim->qmodel_setdata(managedHandle, index, value, role); }
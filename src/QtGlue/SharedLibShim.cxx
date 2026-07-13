#include <QtGlobal>
#include "Base.cxx"
#include "CVariant.cxx"
#include "CModelIndex.cxx"

typedef void *CReversePInvokeShimPtr;
typedef void *GCHandle;

struct CReversePInvokeShim : public CStruct<CReversePInvokeShimPtr>
{
    CReversePInvokeShim(CReversePInvokeShimPtr pointer) : CStruct(pointer) {}

    auto RouteSignalCore() const { return *Get<void (*)(QObject *, const char *)>(0); }
    auto InvokeCallbackCore() const { return *Get<void (*)(void *, void *)>(sizeof(void *)); }
    auto qmodel_rowcount() const { return *Get<qint32 (*)(GCHandle, CModelIndexPtr)>(sizeof(void *) * 2); }
    auto qmodel_data() const { return *Get<CVariantPtr (*)(GCHandle, CModelIndexPtr, qint32)>(sizeof(void *) * 3); }
    auto qmodel_headerdata() const { return *Get<CVariantPtr (*)(GCHandle, qint32, qint32, qint32)>(sizeof(void *) * 4); }
    auto qmodel_columncount() const { return *Get<qint32 (*)(GCHandle, CModelIndexPtr)>(sizeof(void *) * 5); }
    auto qmodel_index() const { return *Get<CModelIndexPtr (*)(GCHandle, qint32, qint32, CModelIndexPtr)>(sizeof(void *) * 6); }
    auto qmodel_parent() const { return *Get<CModelIndexPtr (*)(GCHandle, CModelIndexPtr)>(sizeof(void *) * 7); }
    auto qmodel_flags() const { return *Get<qint32 (*)(GCHandle, CModelIndexPtr)>(sizeof(void *) * 8); }
    auto qmodel_setdata() const { return *Get<qint32 (*)(GCHandle, CModelIndexPtr, CVariantPtr, qint32)>(sizeof(void *) * 9); }
};

CReversePInvokeShim *shim;

extern "C" void InitializeShim(CReversePInvokeShimPtr pointer)
{
    shim = new CReversePInvokeShim(pointer);
}

extern "C" void RouteSignalCore(QObject *sender, const char *signalName) { shim->RouteSignalCore()(sender, signalName); }
extern "C" void InvokeCallbackCore(void *callbackHandle, void *stateHandle) { shim->InvokeCallbackCore()(callbackHandle, stateHandle); }
extern "C" qint32 qmodel_rowcount(GCHandle managedHandle, CModelIndexPtr parent) { return shim->qmodel_rowcount()(managedHandle, parent); }
extern "C" CVariantPtr qmodel_data(GCHandle managedHandle, CModelIndexPtr index, qint32 role) { return shim->qmodel_data()(managedHandle, index, role); }
extern "C" CVariantPtr qmodel_headerdata(GCHandle managedHandle, qint32 section, qint32 orientation, qint32 role) { return shim->qmodel_headerdata()(managedHandle, section, orientation, role); }
extern "C" qint32 qmodel_columncount(GCHandle managedHandle, CModelIndexPtr parent) { return shim->qmodel_columncount()(managedHandle, parent); }
extern "C" CModelIndexPtr qmodel_index(GCHandle managedHandle, qint32 row, qint32 column, CModelIndexPtr parent) { return shim->qmodel_index()(managedHandle, row, column, parent); }
extern "C" CModelIndexPtr qmodel_parent(GCHandle managedHandle, CModelIndexPtr index) { return shim->qmodel_parent()(managedHandle, index); }
extern "C" qint32 qmodel_flags(GCHandle managedHandle, CModelIndexPtr index) { return shim->qmodel_flags()(managedHandle, index); }
extern "C" qint32 qmodel_setdata(GCHandle managedHandle, CModelIndexPtr index, CVariantPtr value, qint32 role) { return shim->qmodel_setdata()(managedHandle, index, value, role); }
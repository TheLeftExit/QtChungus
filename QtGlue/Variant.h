#include <QMetaType>

void EncodeQVariant(QVariant variant, QMetaType::Type *type, void **value);
QVariant DecodeQVariant(QMetaType::Type type, void *value);
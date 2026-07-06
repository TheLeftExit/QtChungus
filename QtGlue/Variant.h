#include <QMetaType>

typedef qint128 QManagedVariant;

QManagedVariant EncodeQVariant(QVariant variant);
QVariant DecodeQVariant(QManagedVariant variant);
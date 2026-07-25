#include <QObject>
#include <QMetaObject>
#include <QMetaType>
#include <QMetaProperty>
#include <QAbstractItemView>
#include <QMainWindow>

#include "CVariant.cxx"

const QMetaProperty FindProperty(QObject *target, char *name)
{
    const QMetaObject *metaObject = target->metaObject();
    int indexOfProperty = metaObject->indexOfProperty(name);
    return metaObject->property(indexOfProperty);
}

extern "C"
{
    CVariant GetPropertyCore(QObject *target, char *name)
    {
        return FindProperty(target, name).read(target);
    }

    void SetPropertyCore(QObject *target, char *name, CVariant value)
    {
        FindProperty(target, name).write(target, value);
    }
}

// Qt could've just made them native properties...

#define QTGLUE_FOR_EACH_EX_PROPERTY(F)                        \
    F(QAbstractItemView, model, setModel, QAbstractItemModel) \
    F(QMainWindow, centralWidget, setCentralWidget, QWidget)

extern "C"
{
    QObject *GetPropertyCoreEx(QObject *target, char *name)
    {
        std::string_view propertyName{name};
#define TRY_RETURN(TARGET_TYPE, GETTER_NAME, SETTER_NAME, VALUE_TYPE)                   \
    if (propertyName == #GETTER_NAME && dynamic_cast<TARGET_TYPE *>(target) != nullptr) \
    {                                                                                   \
        return dynamic_cast<TARGET_TYPE *>(target)->GETTER_NAME();                      \
    }
        QTGLUE_FOR_EACH_EX_PROPERTY(TRY_RETURN)
#undef TRY_RETURN
        qFatal("GetPropertyCoreEx called on a unknown type/property pair.");
    }

    void SetPropertyCoreEx(QObject *target, char *name, QObject *value)
    {
        std::string_view propertyName{name};
#define TRY_RETURN(TARGET_TYPE, GETTER_NAME, SETTER_NAME, VALUE_TYPE)                        \
    if (propertyName == #GETTER_NAME && dynamic_cast<TARGET_TYPE *>(target) != nullptr)      \
    {                                                                                        \
        dynamic_cast<TARGET_TYPE *>(target)->SETTER_NAME(dynamic_cast<VALUE_TYPE *>(value)); \
        return;                                                                              \
    }
        QTGLUE_FOR_EACH_EX_PROPERTY(TRY_RETURN)
#undef TRY_RETURN
        qFatal("SetPropertyCoreEx called on a unknown type/property pair.");
    }
}
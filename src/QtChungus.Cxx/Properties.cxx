#include <QObject>
#include <QMetaObject>
#include <QMetaType>
#include <QMetaProperty>
#include <QAbstractItemView>
#include <QMainWindow>
#include <QLayout>

#include "CVariant.cxx"

const QMetaProperty FindProperty(QObject *target, char *name)
{
    const QMetaObject *metaObject = target->metaObject();
    int indexOfProperty = metaObject->indexOfProperty(name);
    if (indexOfProperty == -1)
        return QMetaProperty();
    return metaObject->property(indexOfProperty);
}

// Qt could've just made them native properties...
#define QTGLUE_FOR_EACH_EX_PROPERTY(F)                          \
    F(QAbstractItemView, model, setModel, QAbstractItemModel *) \
    F(QMainWindow, centralWidget, setCentralWidget, QWidget *)  \
    F(QWidget, layout, setLayout, QLayout *)                    \
    F(QBoxLayout, direction, setDirection, QBoxLayout::Direction)

extern "C"
{
    CVariant GetPropertyCore(QObject *target, char *name)
    {
        // Good properties
        QMetaProperty property = FindProperty(target, name);
        if (property.isValid())
        {
            return property.read(target);
        }

        // Bad properties
        std::string_view propertyName{name};
#define TRY_RETURN(TARGET_TYPE, GETTER_NAME, SETTER_NAME, VALUE_TYPE)                               \
    if (propertyName == #GETTER_NAME && dynamic_cast<TARGET_TYPE *>(target) != nullptr)             \
    {                                                                                               \
        return QVariant::fromValue<VALUE_TYPE>(dynamic_cast<TARGET_TYPE *>(target)->GETTER_NAME()); \
    }
        QTGLUE_FOR_EACH_EX_PROPERTY(TRY_RETURN)
#undef TRY_RETURN
        qFatal("GetPropertyCore called on a unknown type/property pair (%s::%s).", target->metaObject()->className(), name);
    }

    void SetPropertyCore(QObject *target, char *name, CVariant value)
    {
        // Good properties
        QMetaProperty property = FindProperty(target, name);
        if (property.isValid())
        {
            property.write(target, value);
            return;
        }

        // Bad properties
        std::string_view propertyName{name};
#define TRY_RETURN(TARGET_TYPE, GETTER_NAME, SETTER_NAME, VALUE_TYPE)                          \
    if (propertyName == #GETTER_NAME && dynamic_cast<TARGET_TYPE *>(target) != nullptr)        \
    {                                                                                          \
        dynamic_cast<TARGET_TYPE *>(target)->SETTER_NAME(QVariant(value).value<VALUE_TYPE>()); \
        return;                                                                                \
    }
        QTGLUE_FOR_EACH_EX_PROPERTY(TRY_RETURN)
#undef TRY_RETURN
        qFatal("SetPropertyCore called on a unknown type/property pair (%s::%s).", target->metaObject()->className(), name);
    }
}
#include <QObject>
#include <QApplication>
#include <QMainWindow>
#include <QPushButton>
#include <QMetaMethod>
#include <QLineEdit>
#include <QLabel>
#include <QTreeView>
#include <QAbstractItemView>
#include <QAbstractItemModel>

#include <string>

extern "C" QWidget *CreateWidget(char *widgetName, QWidget *parent)
{
    std::string_view name{widgetName};
#define TRY_RETURN(typeName) \
    if (name == #typeName)   \
        return new typeName(parent);
    TRY_RETURN(QMainWindow)
    TRY_RETURN(QPushButton)
    TRY_RETURN(QLabel)
    TRY_RETURN(QLineEdit)
    TRY_RETURN(QTreeView)
#undef TRY_RETURN
    qFatal("CreateWidget called on an invalid widget name.");
}

extern "C"
{
    void qobject_delete(QObject *target) { delete target; }

    QApplication *qapplication_new(int *argc, char **argv) { return new QApplication(*argc, argv); }

    int qapplication_exec(QApplication *target) { return target->exec(); }
    void qcoreapplication_exit(QCoreApplication *target) { target->exit(); }
    void qmainwindow_show(QMainWindow *target) { return target->show(); }

    void qabstractitemview_setmodel(QAbstractItemView *target, QAbstractItemModel *model) { target->setModel(model); }
    QAbstractItemModel *qabstractitemview_model(QAbstractItemView *target) { return target->model(); }
}
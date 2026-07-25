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
#include <QHBoxLayout>
#include <QVBoxLayout>
#include <QGridLayout>
#include <QFormLayout>

#include <string>

extern "C" QObject *CreateWidgetCore(char *widgetName, QWidget *parent)
{
    std::string_view name{widgetName};
#define TRY_RETURN(typeName) \
    if (name == #typeName)   \
        return new typeName(parent);
    TRY_RETURN(QWidget)
    TRY_RETURN(QMainWindow)
    TRY_RETURN(QPushButton)
    TRY_RETURN(QLabel)
    TRY_RETURN(QLineEdit)
    TRY_RETURN(QTreeView)
    // not really widgets, but why not?
    TRY_RETURN(QHBoxLayout)
    TRY_RETURN(QVBoxLayout)
    TRY_RETURN(QGridLayout)
    TRY_RETURN(QFormLayout)
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

    void qmainwindow_setcentralwidget(QMainWindow *target, QWidget *widget)
    {
        return target->setCentralWidget(widget);
    }
    QWidget *qmainwindow_centralwidget(QMainWindow *target){
        return target->centralWidget();
    }

    void qboxlayout_addwidget(QBoxLayout *target, QWidget *widget, qint32 stretch, qint32 alignment)
    {
        target->addWidget(widget, stretch, static_cast<Qt::Alignment>(alignment));
    }
    void qboxlayout_addlayout(QBoxLayout *target, QLayout *layout, qint32 stretch)
    {
        target->addLayout(layout, stretch);
    }
    void qboxlayout_addstretch(QBoxLayout *target, qint32 stretch)
    {
        target->addStretch(stretch);
    }
    void qgridlayout_addwidget(QGridLayout *target, QWidget *widget, qint32 row, qint32 column, qint32 rowSpan, qint32 columnSpan, qint32 alignment)
    {
        target->addWidget(widget, row, column, rowSpan, columnSpan, static_cast<Qt::Alignment>(alignment));
    }
    void qgridlayout_addlayout(QGridLayout *target, QLayout *layout, qint32 row, qint32 column, qint32 rowSpan, qint32 columnSpan, qint32 alignment)
    {
        target->addLayout(layout, row, column, rowSpan, columnSpan, static_cast<Qt::Alignment>(alignment));
    }
    void qformlayout_addwidget(QFormLayout *target, char16_t *labelText, QWidget *widget)
    {
        target->addRow(QString(labelText), widget);
    }
    void qformlayout_addlayout(QFormLayout *target, char16_t *labelText, QLayout *layout)
    {
        target->addRow(QString(labelText), layout);
    }
}
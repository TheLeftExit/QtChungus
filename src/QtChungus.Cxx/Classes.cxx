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
#include <QGroupBox>
#include <QTextEdit>

#include <string>

extern "C" QObject *CreateObjectCore(char *className)
{
    std::string_view name{className};
#define TRY_RETURN(TYPE_NAME) \
    if (name == #TYPE_NAME)   \
        return new TYPE_NAME();

    // Widgets
    TRY_RETURN(QWidget)
    TRY_RETURN(QMainWindow)
    TRY_RETURN(QPushButton)
    TRY_RETURN(QLabel)
    TRY_RETURN(QLineEdit)
    TRY_RETURN(QTextEdit)
    TRY_RETURN(QTreeView)
    TRY_RETURN(QGroupBox)
    TRY_RETURN(QFrame)

    // Layouts
    TRY_RETURN(QGridLayout)
    TRY_RETURN(QFormLayout)
    
#undef TRY_RETURN

    if (name == "QBoxLayout") // that's just annoying
        return new QBoxLayout(QBoxLayout::LeftToRight);

    qFatal("CreateObjectCore called on an invalid class name (%s).", className);
}

extern "C"
{
    void qobject_delete(QObject *target) { delete target; }

    QApplication *qapplication_new(int *argc, char **argv) { return new QApplication(*argc, argv); }

    int qapplication_exec(QApplication *target) { return target->exec(); }
    void qcoreapplication_exit(QCoreApplication *target) { target->exit(); }
    void qwidget_show(QWidget *target) { return target->show(); }

    void qboxlayout_addwidget(QBoxLayout *target, QWidget *widget, qint32 stretch, qint32 alignment)
    {
        target->addWidget(widget, stretch, static_cast<Qt::AlignmentFlag>(alignment));
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
        target->addWidget(widget, row, column, rowSpan, columnSpan, static_cast<Qt::AlignmentFlag>(alignment));
    }
    void qgridlayout_addlayout(QGridLayout *target, QLayout *layout, qint32 row, qint32 column, qint32 rowSpan, qint32 columnSpan, qint32 alignment)
    {
        target->addLayout(layout, row, column, rowSpan, columnSpan, static_cast<Qt::AlignmentFlag>(alignment));
    }
    void qformlayout_addwidget(QFormLayout *target, char16_t *labelText, QWidget *widget)
    {
        target->addRow(QString(labelText), widget);
    }
    void qformlayout_addlayout(QFormLayout *target, char16_t *labelText, QLayout *layout)
    {
        target->addRow(QString(labelText), layout);
    }
    void qgridlayout_setcolumnminimumwidth(QGridLayout *target, qint32 column, qint32 minSize)
    {
        target->setColumnMinimumWidth(column, minSize);
    }
    void qgridlayout_setcolumnstretch(QGridLayout *target, qint32 column, qint32 stretch)
    {
        target->setColumnStretch(column, stretch);
    }
    void qgridlayout_setrowminimumheight(QGridLayout *target, qint32 row, qint32 minSize)
    {
        target->setRowMinimumHeight(row, minSize);
    }
    void qgridlayout_setrowstretch(QGridLayout *target, qint32 row, qint32 stretch)
    {
        target->setRowStretch(row, stretch);
    }
}
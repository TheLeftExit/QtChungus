#include "CVariant.cxx"
#include <QObject>
#include <QApplication>
#include <QLayout>
#include <QWidget>
#include <QMainWindow>
#include <QAbstractButton>
#include <QPushButton>
#include <QFrame>
#include <QLabel>
#include <QLineEdit>
#include <QAbstractItemModel>
#include <QAbstractItemView>
#include <QTreeView>
#include <QGroupBox>
#include <QBoxLayout>
#include <QGridLayout>
#include <QFormLayout>
#include "QcAbstractItemModel.h"
#include "QcSignalRouter.h"
#include "QcSynchronizationContext.h"
extern "C" void QcInvoke(const char *methodId, QObject *target, CVariant *returnValue, CVariant *parameters)
{
    std::string_view methodIdView{methodId};
    if (methodIdView == "QAbstractButton_setText_string")
    {
        dynamic_cast<QAbstractButton *>(target)->setText(static_cast<QString>(parameters[0]));
        return;
    }
    if (methodIdView == "QAbstractButton_text")
    {
        *returnValue = dynamic_cast<QAbstractButton *>(target)->text();
        return;
    }
    if (methodIdView == "QAbstractItemView_model")
    {
        *returnValue = dynamic_cast<QAbstractItemView *>(target)->model();
        return;
    }
    if (methodIdView == "QAbstractItemView_setModel_QAbstractItemModel")
    {
        dynamic_cast<QAbstractItemView *>(target)->setModel(dynamic_cast<QAbstractItemModel *>(static_cast<QObject *>(parameters[0])));
        return;
    }
    if (methodIdView == "QApplication_@new_int*_byte**")
    {
        *returnValue = new QApplication(*static_cast<int *>(static_cast<void *>(parameters[0])), static_cast<char **>(static_cast<void *>(parameters[1])));
        return;
    }
    if (methodIdView == "QApplication_exec")
    {
        *returnValue = dynamic_cast<QApplication *>(target)->exec();
        return;
    }
    if (methodIdView == "QApplication_exit_int")
    {
        dynamic_cast<QApplication *>(target)->exit(static_cast<qint32>(parameters[0]));
        return;
    }
    if (methodIdView == "QBoxLayout_@new_QBoxLayoutDirection")
    {
        *returnValue = new QBoxLayout(static_cast<QBoxLayout::Direction>(static_cast<qint64>(parameters[0])));
        return;
    }
    if (methodIdView == "QBoxLayout_addLayout_QLayout_int")
    {
        dynamic_cast<QBoxLayout *>(target)->addLayout(dynamic_cast<QLayout *>(static_cast<QObject *>(parameters[0])), static_cast<qint32>(parameters[1]));
        return;
    }
    if (methodIdView == "QBoxLayout_addStretch_int")
    {
        dynamic_cast<QBoxLayout *>(target)->addStretch(static_cast<qint32>(parameters[0]));
        return;
    }
    if (methodIdView == "QBoxLayout_addWidget_QWidget_int_QtAlignment")
    {
        dynamic_cast<QBoxLayout *>(target)->addWidget(dynamic_cast<QWidget *>(static_cast<QObject *>(parameters[0])), static_cast<qint32>(parameters[1]), static_cast<Qt::AlignmentFlag>(static_cast<qint64>(parameters[2])));
        return;
    }
    if (methodIdView == "QcAbstractItemModel_@new_nint_nint")
    {
        *returnValue = new QcAbstractItemModel(static_cast<void*>(parameters[0]), static_cast<void*>(parameters[1]));
        return;
    }
    if (methodIdView == "QcAbstractItemModel_emitDataChanged_CModelIndex_CModelIndex")
    {
        dynamic_cast<QcAbstractItemModel *>(target)->emitDataChanged(parameters[0].ToCustomType<CModelIndex>(), parameters[1].ToCustomType<CModelIndex>());
        return;
    }
    if (methodIdView == "QcAbstractItemModel_managedHandle")
    {
        *returnValue = dynamic_cast<QcAbstractItemModel *>(target)->managedHandle();
        return;
    }
    if (methodIdView == "QcSignalRouter_@new_delegate* unmanaged<QObjectStar, byte*, void>")
    {
        *returnValue = new QcSignalRouter(reinterpret_cast<void (*)(QObject *, const char *)>(static_cast<void *>(parameters[0])));
        return;
    }
    if (methodIdView == "QcSignalRouter_ConnectEventsCore_QObjectStar")
    {
        dynamic_cast<QcSignalRouter *>(target)->ConnectEventsCore(static_cast<QObject *>(parameters[0]));
        return;
    }
    if (methodIdView == "QcSignalRouter_ConnectSignalCore_QObjectStar_byte*")
    {
        dynamic_cast<QcSignalRouter *>(target)->ConnectSignalCore(static_cast<QObject *>(parameters[0]), static_cast<char *>(static_cast<void *>(parameters[1])));
        return;
    }
    if (methodIdView == "QcSynchronizationContext_@new_delegate* unmanaged<nint, nint, void>")
    {
        *returnValue = new QcSynchronizationContext(reinterpret_cast<void (*)(void *, void *)>(static_cast<void *>(parameters[0])));
        return;
    }
    if (methodIdView == "QcSynchronizationContext_PostCallback_nint_nint")
    {
        dynamic_cast<QcSynchronizationContext *>(target)->PostCallback(static_cast<void*>(parameters[0]), static_cast<void*>(parameters[1]));
        return;
    }
    if (methodIdView == "QFormLayout_@new")
    {
        *returnValue = new QFormLayout();
        return;
    }
    if (methodIdView == "QFormLayout_addRow_string_QLayout")
    {
        dynamic_cast<QFormLayout *>(target)->addRow(static_cast<QString>(parameters[0]), dynamic_cast<QLayout *>(static_cast<QObject *>(parameters[1])));
        return;
    }
    if (methodIdView == "QFormLayout_addRow_string_QWidget")
    {
        dynamic_cast<QFormLayout *>(target)->addRow(static_cast<QString>(parameters[0]), dynamic_cast<QWidget *>(static_cast<QObject *>(parameters[1])));
        return;
    }
    if (methodIdView == "QGridLayout_@new")
    {
        *returnValue = new QGridLayout();
        return;
    }
    if (methodIdView == "QGridLayout_addLayout_QLayout_int_int_int_int_QtAlignment")
    {
        dynamic_cast<QGridLayout *>(target)->addLayout(dynamic_cast<QLayout *>(static_cast<QObject *>(parameters[0])), static_cast<qint32>(parameters[1]), static_cast<qint32>(parameters[2]), static_cast<qint32>(parameters[3]), static_cast<qint32>(parameters[4]), static_cast<Qt::AlignmentFlag>(static_cast<qint64>(parameters[5])));
        return;
    }
    if (methodIdView == "QGridLayout_addWidget_QWidget_int_int_int_int_QtAlignment")
    {
        dynamic_cast<QGridLayout *>(target)->addWidget(dynamic_cast<QWidget *>(static_cast<QObject *>(parameters[0])), static_cast<qint32>(parameters[1]), static_cast<qint32>(parameters[2]), static_cast<qint32>(parameters[3]), static_cast<qint32>(parameters[4]), static_cast<Qt::AlignmentFlag>(static_cast<qint64>(parameters[5])));
        return;
    }
    if (methodIdView == "QGridLayout_setColumnMinimumWidth_int_int")
    {
        dynamic_cast<QGridLayout *>(target)->setColumnMinimumWidth(static_cast<qint32>(parameters[0]), static_cast<qint32>(parameters[1]));
        return;
    }
    if (methodIdView == "QGridLayout_setColumnStretch_int_int")
    {
        dynamic_cast<QGridLayout *>(target)->setColumnStretch(static_cast<qint32>(parameters[0]), static_cast<qint32>(parameters[1]));
        return;
    }
    if (methodIdView == "QGridLayout_setRowMinimumHeight_int_int")
    {
        dynamic_cast<QGridLayout *>(target)->setRowMinimumHeight(static_cast<qint32>(parameters[0]), static_cast<qint32>(parameters[1]));
        return;
    }
    if (methodIdView == "QGridLayout_setRowStretch_int_int")
    {
        dynamic_cast<QGridLayout *>(target)->setRowStretch(static_cast<qint32>(parameters[0]), static_cast<qint32>(parameters[1]));
        return;
    }
    if (methodIdView == "QGroupBox_@new")
    {
        *returnValue = new QGroupBox();
        return;
    }
    if (methodIdView == "QGroupBox_setTitle_string")
    {
        dynamic_cast<QGroupBox *>(target)->setTitle(static_cast<QString>(parameters[0]));
        return;
    }
    if (methodIdView == "QGroupBox_title")
    {
        *returnValue = dynamic_cast<QGroupBox *>(target)->title();
        return;
    }
    if (methodIdView == "QLabel_@new")
    {
        *returnValue = new QLabel();
        return;
    }
    if (methodIdView == "QLabel_setText_string")
    {
        dynamic_cast<QLabel *>(target)->setText(static_cast<QString>(parameters[0]));
        return;
    }
    if (methodIdView == "QLabel_text")
    {
        *returnValue = dynamic_cast<QLabel *>(target)->text();
        return;
    }
    if (methodIdView == "QLineEdit_@new")
    {
        *returnValue = new QLineEdit();
        return;
    }
    if (methodIdView == "QLineEdit_placeholderText")
    {
        *returnValue = dynamic_cast<QLineEdit *>(target)->placeholderText();
        return;
    }
    if (methodIdView == "QLineEdit_setPlaceholderText_string")
    {
        dynamic_cast<QLineEdit *>(target)->setPlaceholderText(static_cast<QString>(parameters[0]));
        return;
    }
    if (methodIdView == "QLineEdit_setText_string")
    {
        dynamic_cast<QLineEdit *>(target)->setText(static_cast<QString>(parameters[0]));
        return;
    }
    if (methodIdView == "QLineEdit_text")
    {
        *returnValue = dynamic_cast<QLineEdit *>(target)->text();
        return;
    }
    if (methodIdView == "QMainWindow_@new")
    {
        *returnValue = new QMainWindow();
        return;
    }
    if (methodIdView == "QMainWindow_centralWidget")
    {
        *returnValue = dynamic_cast<QMainWindow *>(target)->centralWidget();
        return;
    }
    if (methodIdView == "QMainWindow_setCentralWidget_QWidget")
    {
        dynamic_cast<QMainWindow *>(target)->setCentralWidget(dynamic_cast<QWidget *>(static_cast<QObject *>(parameters[0])));
        return;
    }
    if (methodIdView == "QPushButton_@new")
    {
        *returnValue = new QPushButton();
        return;
    }
    if (methodIdView == "QTreeView_rootIsDecorated")
    {
        *returnValue = dynamic_cast<QTreeView *>(target)->rootIsDecorated();
        return;
    }
    if (methodIdView == "QTreeView_setRootIsDecorated_bool")
    {
        dynamic_cast<QTreeView *>(target)->setRootIsDecorated(static_cast<bool>(parameters[0]));
        return;
    }
    if (methodIdView == "QWidget_@new")
    {
        *returnValue = new QWidget();
        return;
    }
    if (methodIdView == "QWidget_layout")
    {
        *returnValue = dynamic_cast<QWidget *>(target)->layout();
        return;
    }
    if (methodIdView == "QWidget_setLayout_QLayout")
    {
        dynamic_cast<QWidget *>(target)->setLayout(dynamic_cast<QLayout *>(static_cast<QObject *>(parameters[0])));
        return;
    }
    if (methodIdView == "QWidget_show")
    {
        dynamic_cast<QWidget *>(target)->show();
        return;
    }
    qFatal("Unknown methodId: %s, methodId");
}

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
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QGridLayout>
#include <QFormLayout>
#include "QcAbstractItemModel.hxx"
#include "QcSignalRouter.hxx"
#include "QcSynchronizationContext.hxx"
extern "C" void QcInvoke(int methodId, QObject *target, CVariant *returnValue, CVariant *parameters)
{
    switch (methodId)
    {
    case 0:
        dynamic_cast<QAbstractButton *>(target)->setText(static_cast<QString>(parameters[0]));
        return;
    case 1:
        *returnValue = dynamic_cast<QAbstractButton *>(target)->text();
        return;
    case 2:
        *returnValue = dynamic_cast<QAbstractItemView *>(target)->model();
        return;
    case 3:
        dynamic_cast<QAbstractItemView *>(target)->setModel(dynamic_cast<QAbstractItemModel *>(static_cast<QObject *>(parameters[0])));
        return;
    case 4:
        *returnValue = new QApplication(*static_cast<int *>(static_cast<void *>(parameters[0])), static_cast<char **>(static_cast<void *>(parameters[1])));
        return;
    case 5:
        *returnValue = dynamic_cast<QApplication *>(target)->exec();
        return;
    case 6:
        dynamic_cast<QApplication *>(target)->exit(static_cast<qint32>(parameters[0]));
        return;
    case 7:
        *returnValue = dynamic_cast<QApplication *>(target)->quitOnLastWindowClosed();
        return;
    case 8:
        dynamic_cast<QApplication *>(target)->setQuitOnLastWindowClosed(static_cast<bool>(parameters[0]));
        return;
    case 9:
        *returnValue = new QBoxLayout(static_cast<QBoxLayout::Direction>(static_cast<qint64>(parameters[0])));
        return;
    case 10:
        dynamic_cast<QBoxLayout *>(target)->addLayout(dynamic_cast<QLayout *>(static_cast<QObject *>(parameters[0])), static_cast<qint32>(parameters[1]));
        return;
    case 11:
        dynamic_cast<QBoxLayout *>(target)->addStretch(static_cast<qint32>(parameters[0]));
        return;
    case 12:
        dynamic_cast<QBoxLayout *>(target)->addWidget(dynamic_cast<QWidget *>(static_cast<QObject *>(parameters[0])), static_cast<qint32>(parameters[1]), static_cast<Qt::AlignmentFlag>(static_cast<qint64>(parameters[2])));
        return;
    case 13:
        *returnValue = new QcAbstractItemModel(static_cast<void*>(parameters[0]), static_cast<void*>(parameters[1]));
        return;
    case 14:
        dynamic_cast<QcAbstractItemModel *>(target)->emitDataChanged(parameters[0].ToCustomType<CModelIndex>(), parameters[1].ToCustomType<CModelIndex>());
        return;
    case 15:
        *returnValue = dynamic_cast<QcAbstractItemModel *>(target)->managedHandle();
        return;
    case 16:
        *returnValue = new QcSignalRouter(reinterpret_cast<void (*)(QObject *, const char *)>(static_cast<void *>(parameters[0])));
        return;
    case 17:
        dynamic_cast<QcSignalRouter *>(target)->connectEventsCore(static_cast<QObject *>(parameters[0]));
        return;
    case 18:
        dynamic_cast<QcSignalRouter *>(target)->connectSignalCore(static_cast<QObject *>(parameters[0]), static_cast<char *>(static_cast<void *>(parameters[1])));
        return;
    case 19:
        *returnValue = new QcSynchronizationContext(reinterpret_cast<void (*)(void *, void *)>(static_cast<void *>(parameters[0])));
        return;
    case 20:
        dynamic_cast<QcSynchronizationContext *>(target)->postCallback(static_cast<void*>(parameters[0]), static_cast<void*>(parameters[1]));
        return;
    case 21:
        *returnValue = new QFormLayout();
        return;
    case 22:
        dynamic_cast<QFormLayout *>(target)->addRow(static_cast<QString>(parameters[0]), dynamic_cast<QLayout *>(static_cast<QObject *>(parameters[1])));
        return;
    case 23:
        dynamic_cast<QFormLayout *>(target)->addRow(static_cast<QString>(parameters[0]), dynamic_cast<QWidget *>(static_cast<QObject *>(parameters[1])));
        return;
    case 24:
        *returnValue = new QFrame();
        return;
    case 25:
        *returnValue = new QGridLayout();
        return;
    case 26:
        dynamic_cast<QGridLayout *>(target)->addLayout(dynamic_cast<QLayout *>(static_cast<QObject *>(parameters[0])), static_cast<qint32>(parameters[1]), static_cast<qint32>(parameters[2]), static_cast<qint32>(parameters[3]), static_cast<qint32>(parameters[4]), static_cast<Qt::AlignmentFlag>(static_cast<qint64>(parameters[5])));
        return;
    case 27:
        dynamic_cast<QGridLayout *>(target)->addWidget(dynamic_cast<QWidget *>(static_cast<QObject *>(parameters[0])), static_cast<qint32>(parameters[1]), static_cast<qint32>(parameters[2]), static_cast<qint32>(parameters[3]), static_cast<qint32>(parameters[4]), static_cast<Qt::AlignmentFlag>(static_cast<qint64>(parameters[5])));
        return;
    case 28:
        dynamic_cast<QGridLayout *>(target)->setColumnMinimumWidth(static_cast<qint32>(parameters[0]), static_cast<qint32>(parameters[1]));
        return;
    case 29:
        dynamic_cast<QGridLayout *>(target)->setColumnStretch(static_cast<qint32>(parameters[0]), static_cast<qint32>(parameters[1]));
        return;
    case 30:
        dynamic_cast<QGridLayout *>(target)->setRowMinimumHeight(static_cast<qint32>(parameters[0]), static_cast<qint32>(parameters[1]));
        return;
    case 31:
        dynamic_cast<QGridLayout *>(target)->setRowStretch(static_cast<qint32>(parameters[0]), static_cast<qint32>(parameters[1]));
        return;
    case 32:
        *returnValue = new QGroupBox();
        return;
    case 33:
        dynamic_cast<QGroupBox *>(target)->setTitle(static_cast<QString>(parameters[0]));
        return;
    case 34:
        *returnValue = dynamic_cast<QGroupBox *>(target)->title();
        return;
    case 35:
        *returnValue = new QHBoxLayout();
        return;
    case 36:
        *returnValue = new QLabel();
        return;
    case 37:
        dynamic_cast<QLabel *>(target)->setText(static_cast<QString>(parameters[0]));
        return;
    case 38:
        *returnValue = dynamic_cast<QLabel *>(target)->text();
        return;
    case 39:
        *returnValue = new QLineEdit();
        return;
    case 40:
        *returnValue = dynamic_cast<QLineEdit *>(target)->placeholderText();
        return;
    case 41:
        dynamic_cast<QLineEdit *>(target)->setPlaceholderText(static_cast<QString>(parameters[0]));
        return;
    case 42:
        dynamic_cast<QLineEdit *>(target)->setText(static_cast<QString>(parameters[0]));
        return;
    case 43:
        *returnValue = dynamic_cast<QLineEdit *>(target)->text();
        return;
    case 44:
        *returnValue = new QMainWindow();
        return;
    case 45:
        *returnValue = dynamic_cast<QMainWindow *>(target)->centralWidget();
        return;
    case 46:
        dynamic_cast<QMainWindow *>(target)->resize(static_cast<QSize>(parameters[0]));
        return;
    case 47:
        dynamic_cast<QMainWindow *>(target)->setCentralWidget(dynamic_cast<QWidget *>(static_cast<QObject *>(parameters[0])));
        return;
    case 48:
        dynamic_cast<QMainWindow *>(target)->setWindowTitle(static_cast<QString>(parameters[0]));
        return;
    case 49:
        *returnValue = dynamic_cast<QMainWindow *>(target)->size();
        return;
    case 50:
        *returnValue = dynamic_cast<QMainWindow *>(target)->windowTitle();
        return;
    case 51:
        *returnValue = new QPushButton();
        return;
    case 52:
        *returnValue = new QTreeView();
        return;
    case 53:
        *returnValue = dynamic_cast<QTreeView *>(target)->rootIsDecorated();
        return;
    case 54:
        dynamic_cast<QTreeView *>(target)->setRootIsDecorated(static_cast<bool>(parameters[0]));
        return;
    case 55:
        *returnValue = new QVBoxLayout();
        return;
    case 56:
        *returnValue = new QWidget();
        return;
    case 57:
        *returnValue = dynamic_cast<QWidget *>(target)->isVisible();
        return;
    case 58:
        *returnValue = dynamic_cast<QWidget *>(target)->layout();
        return;
    case 59:
        dynamic_cast<QWidget *>(target)->setLayout(dynamic_cast<QLayout *>(static_cast<QObject *>(parameters[0])));
        return;
    case 60:
        dynamic_cast<QWidget *>(target)->setVisible(static_cast<bool>(parameters[0]));
        return;
    case 61:
        dynamic_cast<QWidget *>(target)->show();
        return;
    default:
        qFatal("Unknown methodId: %s, methodId");
    }
}

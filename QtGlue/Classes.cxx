#include <QObject>
#include <QApplication>
#include <QMainWindow>
#include <QPushButton>
#include <QMetaMethod>
#include <QTextEdit>

#include <iostream>

typedef struct
{
    int x, y, w, h;
} Rectangle;

extern "C"
{
    QApplication *qapplication_new(int *argc, char **argv) { return new QApplication(*argc, argv); }
    int qapplication_exec(QApplication *target) { return target->exec(); }
    void qapplication_delete(QApplication *target) { delete target; }
    void qguiapplication_setquitonlastwindowclosed(QGuiApplication *target, int32_t value)
    {
        target->setQuitOnLastWindowClosed(value);
    }
    void qcoreapplication_exit(QCoreApplication *target) { target->exit(); }

    QMainWindow *qmainwindow_new() { return new QMainWindow(); }
    void qmainwindow_show(QMainWindow *target) { return target->show(); }
    void qmainwindow_delete(QMainWindow *target) { delete target; }

    QPushButton *qpushbutton_new(QWidget *parent) { return new QPushButton(parent); }

    void qabstractbutton_settext(QAbstractButton *target, char16_t *value) { target->setText(QString(value)); }
    const ushort *qabstractbutton_text(QAbstractButton *target) { return target->text().utf16(); }

    void qwidget_setgeometry(QWidget *target, Rectangle r) { target->setGeometry(r.x, r.y, r.w, r.h); }
    Rectangle qwidget_geometry(QWidget *target)
    {
        QRect g = target->geometry();
        return Rectangle{g.x(), g.y(), g.width(), g.height()};
    }
    int32_t qwidget_isvisible(QWidget *target) { return target->isVisible(); }
}
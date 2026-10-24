using System.IO.Compression;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;

namespace QtChungus;

public enum ApplicationState { NotCreated, Created, Running, Finished, Disposed }

public partial class QcApplication : IDisposable
{
    public static QcApplication? Instance { get; private set; }
    public static ApplicationState State { get; private set; }

    private QApplication _instance;




    public QcApplication()
    {
        if (State != ApplicationState.NotCreated) throw new InvalidOperationException();

        _instance = CreateQApplication();
        Instance = this;
        State = ApplicationState.Created;

    }
    public int Run()
    {
        if (State != ApplicationState.Created) throw new InvalidOperationException();

        State = ApplicationState.Running;
        var oldSc = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new QcSynchronizationContext());
        var result = _instance.exec();
        SynchronizationContext.SetSynchronizationContext(oldSc);
        State = ApplicationState.Finished;

        return result;
    }
    public void Dispose()
    {
        if (State is not ApplicationState.Created and not ApplicationState.Finished) throw new InvalidOperationException();

        Qt.delete(_instance.Handle);

        State = ApplicationState.Disposed;
    }

    public static void Run(Func<Task<int>> mainFunc)
    {
        using var application = new QcApplication();
        var handle = application._instance;
        handle.QuitOnLastWindowClosed = false;
        mainFunc().ContinueWith(task =>
        {
            if (task.Exception is not null)
            {
                Environment.FailFast(null, task.Exception);
            }

            Qt.QApplication_exit(handle, task.Result);
        }, TaskContinuationOptions.ExecuteSynchronously);
        application.Run();
    }

    public static void Run(Func<Task> mainFunc) => Run(async () =>
    {
        await mainFunc();
        return 0;
    });
}

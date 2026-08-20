using System.IO.Compression;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;

namespace QtChungus;

public enum ApplicationState { NotCreated, Created, Running, Finished, Disposed }

public partial class Application : IDisposable
{
    public static Application? Instance { get; private set; }
    public static ApplicationState State { get; private set; }

    private QObject _instance;

    private unsafe QObject CreateQApplication()
    {
        var args = Environment.GetCommandLineArgs();
        var argv = (byte**)NativeMemory.Alloc((nuint)args.Length, (nuint)sizeof(byte*));
        for (int i = 0; i < args.Length; i++)
        {
            var utf16StringSpan = args[i].AsSpan();

            var utf8StringLength = Encoding.UTF8.GetByteCount(utf16StringSpan) + 1;
            var utf8String = (byte*)NativeMemory.Alloc((nuint)utf8StringLength);
            var utf8StringSpan = new Span<byte>(utf8String, utf8StringLength);

            Encoding.UTF8.GetBytes(utf16StringSpan, utf8StringSpan);
            utf8StringSpan[^1] = 0;

            argv[i] = utf8String;
        }
        var argc = (int*)NativeMemory.Alloc(sizeof(int));
        *argc = args.Length;
        return Qt.qapplication_new(argc, argv);
    }


    public Application()
    {
        if (State != ApplicationState.NotCreated) throw new InvalidOperationException();

        CReversePInvokeShim.Initialize();

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
        var result = Qt.qapplication_exec(_instance);
        SynchronizationContext.SetSynchronizationContext(oldSc);
        State = ApplicationState.Finished;

        return result;
    }
    public void Dispose()
    {
        if (State is not ApplicationState.Created and not ApplicationState.Finished) throw new InvalidOperationException();

        Qt.qobject_delete(_instance);

        State = ApplicationState.Disposed;
    }

    public static void Run(Func<Task> mainFunc)
    {
        using var application = new Application();
        var handle = application._instance;
        handle.SetProperty("quitOnLastWindowClosed", false);
        mainFunc().ContinueWith(task =>
        {
            if (task.Exception is not null)
            {
                Environment.FailFast(null, task.Exception);
            }

            Qt.qcoreapplication_exit(handle);
        }, TaskContinuationOptions.ExecuteSynchronously);
        application.Run();
    }
}

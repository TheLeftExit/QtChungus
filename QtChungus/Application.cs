using System.IO.Compression;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic;

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
#pragma warning disable CA2014 // CLI arguments probably won't overflow the stack..?
        for (int i = 0; i < args.Length; i++)
        {
            var utf16StringLength = args[i].Length + 1;
            var utf16String = stackalloc char[utf16StringLength];
            var utf16StringSpan = new Span<char>(utf16String, utf16StringLength);

            args[i].CopyTo(utf16StringSpan);

            var utf8StringLength = Encoding.UTF8.GetByteCount(utf16StringSpan);
            var utf8String = (byte*)NativeMemory.Alloc((nuint)utf8StringLength);
            var utf8StringSpan = new Span<byte>(utf8String, utf8StringLength);

            Encoding.UTF8.GetBytes(utf16StringSpan, utf8StringSpan);

            argv[i] = utf8String;
        }
#pragma warning restore CA2014
        var argc = (int*)NativeMemory.Alloc(sizeof(int));
        *argc = args.Length;
        return Qt.qapplication_new(argc, argv);
    }

    [LibraryImport(Qt.libraryName)]
    private static partial void InitializeRouter();

    public Application()
    {
        if (State != ApplicationState.NotCreated) throw new InvalidOperationException();

        _instance = CreateQApplication();
        Instance = this;
        State = ApplicationState.Created;
        
        InitializeRouter();
        
    }
    public int Run()
    {
        if (State != ApplicationState.Created) throw new InvalidOperationException();

        State = ApplicationState.Running;
        var oldSc = SynchronizationContext.Current;
        SynchronizationContext.SetSynchronizationContext(new QtSynchronizationContext());
        var result = Qt.qapplication_exec(_instance);
        SynchronizationContext.SetSynchronizationContext(oldSc);
        State = ApplicationState.Finished;

        return result;
    }
    public void Dispose()
    {
        if (State is not ApplicationState.Created and not ApplicationState.Finished) throw new InvalidOperationException();

        Qt.qapplication_delete(_instance);

        State = ApplicationState.Disposed;
    }

    public static void Run(Func<Task> mainFunc)
    {
        using var application = new Application();
        var handle = application._instance;
        handle.SetProperty("quitOnLastWindowClosed", false);
        mainFunc().ContinueWith(_ => Qt.qcoreapplication_exit(handle));
        application.Run();
    }
}


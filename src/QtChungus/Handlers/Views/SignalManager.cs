using System.ComponentModel;

namespace QtChungus;

public sealed class SignalManager<TWindow, TViewModel> : IViewBuilder<TViewModel>, IView<TViewModel>
    where TWindow : struct, IQWidget
    where TViewModel : class
{
    private readonly TWindow _window;
    private readonly List<ISignalHandler<TViewModel>> _signalHandlers;

    public SignalManager(TWindow window)
    {
        _window = window;
        _signalHandlers = new();
    }

    public void Dispose()
    {
        Qt.qobject_delete(_window.Handle);
    }

    public void AddSignalHandler(ISignalHandler<TViewModel> signalHandler)
    {
        _signalHandlers.Add(signalHandler);
    }

    public async Task ShowAsync(TViewModel viewModel)
    {
        var signalReceiver = new ViewSignalReceiver(_signalHandlers, viewModel);

        var bindableViewModel = viewModel as INotifyPropertyChanged;
        if (bindableViewModel is not null)
        {
            bindableViewModel.PropertyChanged += signalReceiver.OnPropertyChanged;
        }

        Qt.qwidget_show(_window.Handle);
        using var signalSubscription = SignalRouter.Subscribe(_window.Handle, signalReceiver);
        signalReceiver.OnPropertyChanged(viewModel, new PropertyChangedEventArgs(null));
        await WhenClosed();

        if (bindableViewModel is not null)
        {
            bindableViewModel.PropertyChanged -= signalReceiver.OnPropertyChanged;
        }
    }

    public async Task WhenClosed()
    {
        var eventReceiver = new ViewCloseEventReceiver(_window.Handle);
        using var closeEventSubscription = SignalRouter.Subscribe(_window.Handle, eventReceiver);
        SignalRouter.ConnectEvents(_window.Handle);
        await eventReceiver.WhenClosed();
    }

    private record ViewSignalReceiver(IEnumerable<ISignalHandler<TViewModel>> SignalHandlers, TViewModel ViewModel) : ISignalReceiver
    {
        public void ProcessSignal(QObject sender, ReadOnlySpan<char> signalName)
        {
            foreach (var handler in SignalHandlers)
            {
                handler.ProcessSignal(sender, signalName, ViewModel!);
            }
        }

        public void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            foreach (var handler in SignalHandlers)
            {
                handler.ProcessPropertyChanged((TViewModel)sender!, e.PropertyName!);
            }
        }
    }

    private record ViewCloseEventReceiver(QObject Handle) : ISignalReceiver
    {
        private readonly TaskCompletionSource _tcs = new();

        public Task WhenClosed()
        {
            return Handle.GetProperty<bool>("visible") ? _tcs.Task : Task.CompletedTask;
        }

        public void ProcessSignal(QObject sender, ReadOnlySpan<char> signalName)
        {
            if (sender == Handle && signalName is "QEvent::Close")
            {
                ProcessCloseEvent();
            }
        }

        private async void ProcessCloseEvent()
        {
            await Task.Yield();
            if (!Handle.GetProperty<bool>("visible"))
            {
                _tcs.SetResult();
            }
        }
    }
}

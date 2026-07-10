using System.ComponentModel;

public class View<TViewModel>
{
    private readonly QObject _handle;
    private readonly ISignalHandler<TViewModel>[] _signalHandlers;

    public View(QObject handle, ISignalHandler<TViewModel>[] signalHandlers)
    {
        (_handle, _signalHandlers) = (handle, signalHandlers);
    }

    public async Task ShowAsync(TViewModel viewModel)
    {
        var signalReceiver = new ViewSignalReceiver(_signalHandlers, viewModel);

        var bindableViewModel = viewModel as INotifyPropertyChanged;
        if (bindableViewModel is not null)
        {
            bindableViewModel.PropertyChanged += signalReceiver.OnPropertyChanged;
        }

        Qt.qmainwindow_show(_handle);
        using var signalSubscription = SignalRouter.Subscribe(_handle, signalReceiver);
        signalReceiver.OnPropertyChanged(viewModel, new PropertyChangedEventArgs(null));
        await WhenClosed();

        if (bindableViewModel is not null)
        {
            bindableViewModel.PropertyChanged -= signalReceiver.OnPropertyChanged;
        }
    }

    public async Task WhenClosed()
    {
        var eventReceiver = new ViewCloseEventReceiver(_handle);
        using var eventSubscription = SignalRouter.Subscribe(_handle, eventReceiver);
        SignalRouter.ConnectEvents(_handle);
        await eventReceiver.WhenClosed();
    }

    private record ViewSignalReceiver(ISignalHandler<TViewModel>[] SignalHandlers, TViewModel ViewModel) : ISignalReceiver
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
        private TaskCompletionSource _tcs = new();

        public Task WhenClosed()
        {
            return Handle.GetProperty<bool>("visible") ? _tcs.Task : Task.CompletedTask;
        }

        public void ProcessSignal(QObject sender, ReadOnlySpan<char> signalName)
        {
            if (sender == Handle && signalName is "QEvent::Close")
            {
                ProcessDestroyed();
            }
        }

        private async void ProcessDestroyed()
        {
            await Task.Yield();
            if (!Handle.GetProperty<bool>("visible"))
            {
                _tcs.SetResult();
            }
        }
    }
}
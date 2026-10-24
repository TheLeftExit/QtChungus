namespace QtChungus;

public abstract class ViewBase<TWindow, TViewModel> : IView<TViewModel>
    where TWindow : struct, IQWidget
    where TViewModel : class
{
    private readonly SignalManager<TWindow, TViewModel> _view;

    protected ViewBase()
    {
        var window = QObjectStar.Create<TWindow>();
        _view = new(window);
        Configure(window, _view);
    }

    protected abstract void Configure(TWindow window, SignalManager<TWindow, TViewModel> view);

    public Task ShowAsync(TViewModel viewModel) => _view.ShowAsync(viewModel);
    public Task WhenClosed() => _view.WhenClosed();
    public void Dispose() => _view.Dispose();
}
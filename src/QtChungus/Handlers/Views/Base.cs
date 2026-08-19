namespace QtChungus;

public interface IView<TViewModel>
    where TViewModel : class
{
    Task ShowAsync(TViewModel viewModel);
    Task WhenClosed();
}

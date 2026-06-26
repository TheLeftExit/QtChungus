using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

[assembly: DisableRuntimeMarshalling]

Application.Run(async () =>
{
    var builder = new ViewBuilder<MyViewModel>();
    var button = builder.AddWidget<QPushButton>(x =>
    {
        x.Bounds = new(10, 10, 120, 30);
        x.Text = "Click me!";
    });
    var textEdit = builder.AddWidget<QLineEdit>(x =>
    {
        x.Bounds = new(10, 50, 120, 30);
        x.PlaceholderText = "Type something!";
    });
    var focusDummy = builder.AddWidget<QTreeView>(x =>
    {
        x.Bounds = new(10, 90, 120, 120); 
    });
    builder.SetBinding(button, x => x.Text, x => x.ButtonText);
    builder.SetBinding(textEdit, x => x.Text, x => x.ButtonText);
    builder.SetCommand(button, x => x.OnClicked, x => x.Fire);
    var view = builder.Build();

    await view.ShowAsync(new MyViewModel());
});

public class MyViewModel : BindableBase
{
    private int counter = 0;
    public string? ButtonText
    {
        get => field;
        set => SetValue(ref field, value);
    }

    public void Fire()
    {
        ButtonText = $"Clicked {++counter} times";
    }
}

public class BindableBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> eventArgsCache = new();

    protected void RaisePropertyChanged(string? propertyName = null)
    {
        var args = eventArgsCache.GetOrAdd(propertyName ?? "$null", x => new PropertyChangedEventArgs(x));
        PropertyChanged?.Invoke(this, args);
    }
    protected void SetValue<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        RaisePropertyChanged(propertyName);
    }
}



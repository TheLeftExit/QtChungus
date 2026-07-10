using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;

[assembly: DisableRuntimeMarshalling]

Console.WriteLine("Hello world!");

Application.Run(async () =>
{
    var builder = new QMainWindowBuilder<MyViewModel>(x =>
    {
        x.Size = new(220, 300);
        x.Text = "QtChungus";
    });
    var rootLayout = builder.UseLayout<QVBoxLayout>();
    var row1Layout = rootLayout.AddLayout<QHBoxLayout>();
    var button = row1Layout.AddWidget<QPushButton>(x =>
    {
        x.Text = "Click me!";
    });
    var textEdit = row1Layout.AddWidget<QLineEdit>(x =>
    {
        x.PlaceholderText = "Type something!";
    });
    var treeView = rootLayout.AddWidget<QTreeView>();
    builder.SetBinding(button, x => x.Text, x => x.ButtonText);
    builder.SetBinding(textEdit, x => x.Text, x => x.ButtonText);
    builder.SetCommand(button, x => x.OnClicked, x => x.Fire);
    builder.SetModel(treeView, x => x.List, [
        new QModelColumn<MyClass, string?>(x => x.Value),
        new QModelColumn<MyClass, bool>(x => x.Checked)
    ]);
    var view = builder.Build();

    await view.ShowAsync(new MyViewModel());
});

public class MyClass
{
    public string? Value { get; set; }
    public bool Checked { get; set; }
}

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
        //throw new NotImplementedException();
    }

    public MyClass[] List { get; } = Enumerable.Range(0, 5).Select(x => new MyClass { Value = x.ToString() }).ToArray();
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



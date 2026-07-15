Application.Run(async () =>
{
    var builder = new QMainWindowBuilder<MyViewModel>(x =>
    {
        x.Size = new(500, 300);
        x.WindowTitle = "QtChungus Demo Application";
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
using System.Drawing;
using QtChungus;
using QtChungus.Internal;

Application.Run(async () =>
{
    var mainWindow = QObject.Create<QMainWindow>() with
    {
        Size = new(500, 300),
#if DEBUG
        WindowTitle = "QtChungus.Demo (DEBUG)",
#else
        WindowTitle = "QtChungus.Demo (RELEASE)",
#endif
    };
    var rootLayout = mainWindow.UseLayout<QBoxLayout>() with { Direction = QBoxLayoutDirection.TopToBottom };
    var row1Layout = rootLayout.AddLayout<QBoxLayout>() with { Direction = QBoxLayoutDirection.LeftToRight };
    var button = row1Layout.AddWidget<QPushButton>() with 
    {
        Text = "Click me!"
    };
    var textEdit = row1Layout.AddWidget<QLineEdit>();
    textEdit.PlaceholderText = "Type something!";

    var treeView = rootLayout.AddWidget<QTreeView>();

    var view = new SignalManager<QMainWindow, MyViewModel>(mainWindow);
    view.SetBinding(button, x => x.Text, x => x.ButtonText);
    view.SetBinding(textEdit, x => x.Text, x => x.ButtonText);
    view.SetCommand(button, x => x.OnClicked, x => x.Fire);
    view.SetModel(treeView, x => x.List, x =>
    {
        x.AddColumn(row => row.Value);
        x.AddColumn(row => row.ValueDouble);
        x.AddColumn(row => row.Checked);
    });

    await view.ShowAsync(new MyViewModel());
});

public class MyClass
{
    public string? Value { get; set; }
    public double ValueDouble { get; set; }
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
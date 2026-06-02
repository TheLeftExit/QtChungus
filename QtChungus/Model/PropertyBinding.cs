using System.Linq.Expressions;
using System.Reflection;

public class PropertyBinding<TViewModel, TWidget, TValue> : ISignalHandler<TViewModel>
    where TViewModel : class
    where TWidget : struct, IWidget<TWidget>
{
    public required QObject WidgetHandle { private get; init; }
    public required string? SignalName { private get; init; }
    public required WidgetGetMethod<TWidget, TValue> WidgetGetMethod { private get; init; }
    public required WidgetSetMethod<TWidget, TValue>? WidgetSetMethod { private get; init; }

    public required string? ViewModelPropertyName { get; set; }
    public required ViewModelGetMethod<TViewModel, TValue> ViewModelGetMethod { private get; init; }
    public required ViewModelSetMethod<TViewModel, TValue>? ViewModelSetMethod { private get; init; }

    public void ProcessSignal(QObject sender, ReadOnlySpan<char> name, TViewModel viewModel)
    {
        if (sender != WidgetHandle) return;
        if (name != SignalName) return;
        Push(viewModel, PushDirection.ToViewModel);
    }

    public void ProcessPropertyChanged(TViewModel sender, string propertyName)
    {
        if (propertyName != ViewModelPropertyName) return;
        Push(sender, PushDirection.ToWidget);
    }

    private enum PushDirection { ToWidget, ToViewModel }
    private void Push(TViewModel viewModel, PushDirection direction)
    {
        if (direction == PushDirection.ToWidget && WidgetSetMethod is null) return;
        if (direction == PushDirection.ToViewModel && ViewModelSetMethod is null) return;

        var widget = new TWidget { Handle = WidgetHandle };

        var widgetValue = WidgetGetMethod(widget);
        var viewModelValue = ViewModelGetMethod(viewModel);
        if (EqualityComparer<TValue>.Default.Equals(widgetValue, viewModelValue)) return;

        switch (direction)
        {
            case PushDirection.ToWidget:
                WidgetSetMethod!(widget, viewModelValue);
                break;
            case PushDirection.ToViewModel:
                ViewModelSetMethod!(viewModel, widgetValue);
                break;
            default:
                throw new InvalidOperationException();
        }
    }
}

public class CommandBinding<TViewModel, TWidget> : ISignalHandler<TViewModel>
    where TViewModel : class
    where TWidget : IWidget<TWidget>
{
    public required QObject WidgetHandle { private get; init; }
    public required string SignalName { private get; init; }
    public required ViewModelCommand<TViewModel> ViewModelCommandMethod { private get; init; }

    public void ProcessSignal(QObject sender, ReadOnlySpan<char> name, TViewModel viewModel)
    {
        if (sender != WidgetHandle) return;
        if (!name.SequenceEqual(SignalName)) return;
        ViewModelCommandMethod(viewModel);
    }

    public void ProcessPropertyChanged(TViewModel sender, string propertyName) { }


}

public static class ViewBuilderExtensions
{
    public static void SetBinding<TViewModel, TWidget, TValue>(
        this IViewBuilder<TViewModel> builder,
        WidgetHandle<TWidget> widget,
        Expression<Func<TWidget, TValue>> controlPropertySelector,
        Expression<Func<TViewModel, TValue>> viewModelPropertySelector
    )
        where TViewModel : class
        where TWidget : struct, IWidget<TWidget>
    {
        if (controlPropertySelector is not { Body: MemberExpression { Member: PropertyInfo controlProperty and { GetMethod: not null } } })
        {
            throw new NotSupportedException();
        }
        if (viewModelPropertySelector is not { Body: MemberExpression { Member: PropertyInfo viewModelProperty and { GetMethod: not null } } })
        {
            throw new NotSupportedException();
        }

        var signalInfo = controlProperty.GetCustomAttribute<QtSignalAttribute>();
        if (signalInfo is not null)
        {
            SignalRouter.ConnectSignal(widget.Handle, signalInfo.SignalSignature);
        }

        var handler = new PropertyBinding<TViewModel, TWidget, TValue>
        {
            WidgetHandle = widget.Handle,
            WidgetGetMethod = controlProperty.GetMethod.CreateDelegate<WidgetGetMethod<TWidget, TValue>>(),
            WidgetSetMethod = controlProperty.SetMethod?.CreateDelegate<WidgetSetMethod<TWidget, TValue>>(),
            SignalName = signalInfo?.SignalName,
            ViewModelGetMethod = viewModelProperty.GetMethod.CreateDelegate<ViewModelGetMethod<TViewModel, TValue>>(),
            ViewModelSetMethod = viewModelProperty.SetMethod?.CreateDelegate<ViewModelSetMethod<TViewModel, TValue>>(),
            ViewModelPropertyName = viewModelProperty.Name
        };
        builder.AddSignalHandler(handler);
    }

    public static void SetCommand<TViewModel, TWidget>(
        this IViewBuilder<TViewModel> builder,
        WidgetHandle<TWidget> widget,
        Expression<Func<TWidget, Action>> controlCommandSelector,
        Expression<Func<TViewModel, Action>> viewModelCommandSelector
    )
        where TViewModel : class
        where TWidget : struct, IWidget<TWidget>
    {
        if (controlCommandSelector is not { Body: UnaryExpression { Operand: MethodCallExpression { Object: ConstantExpression { Value: MethodInfo controlCommand } } } })
        {
            throw new NotSupportedException();
        }
        if (controlCommand.GetCustomAttribute<QtSignalAttribute>() is not QtSignalAttribute signalInfo)
        {
            Console.WriteLine(controlCommand.Name);
            throw new NotSupportedException();
        }
        if (viewModelCommandSelector is not { Body: UnaryExpression { Operand: MethodCallExpression { Object: ConstantExpression { Value: MethodInfo viewModelCommand } } } })
        {
            throw new NotSupportedException();
        }
        if (viewModelCommand.ReturnType != typeof(void) || viewModelCommand.GetParameters().Length != 0)
        {
            throw new NotSupportedException();
        }

        SignalRouter.ConnectSignal(widget.Handle, signalInfo.SignalSignature);

        builder.AddSignalHandler(new CommandBinding<TViewModel, TWidget>
        {
            SignalName = signalInfo.SignalName,
            ViewModelCommandMethod = viewModelCommand.CreateDelegate<ViewModelCommand<TViewModel>>(),
            WidgetHandle = widget.Handle
        });
    }
}

public delegate T WidgetGetMethod<TOwner, T>(in TOwner owner) where TOwner : struct;
public delegate void WidgetSetMethod<TOwner, T>(in TOwner owner, T value) where TOwner : struct;

public delegate T ViewModelGetMethod<TOwner, T>(TOwner owner) where TOwner : class;
public delegate void ViewModelSetMethod<TOwner, T>(TOwner owner, T value) where TOwner : class;

public delegate void ViewModelCommand<TOwner>(TOwner owner) where TOwner : class;
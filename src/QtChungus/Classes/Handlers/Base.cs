using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace QtChungus.Internal;

public interface ISignalHandler<TViewModel>
{
    void ProcessSignal(QObject sender, ReadOnlySpan<char> name, TViewModel viewModel);
    void ProcessPropertyChanged(TViewModel viewModel, string propertyName);
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public sealed class QtSignalAttribute : Attribute
{
    public string SignalName { get; }
    public string SignalSignature { get; }
    public QtSignalAttribute(string signalName, string signalSignature) => (SignalName, SignalSignature) = (signalName, signalSignature);
}

// Despite the name, these are used as generic "class getter/setter"/"struct getter/setter" delegates
public delegate T WidgetGetMethod<TOwner, T>(in TOwner owner) where TOwner : struct;
public delegate void WidgetSetMethod<TOwner, T>(in TOwner owner, T value) where TOwner : struct;

public delegate T ViewModelGetMethod<TOwner, T>(TOwner owner) where TOwner : class;
public delegate void ViewModelSetMethod<TOwner, T>(TOwner owner, T value) where TOwner : class;

public delegate void ViewModelCommand<TOwner>(TOwner owner) where TOwner : class;

public static class ExpressionHelper
{
    public static bool TryParseProperty<TOwner, TValue>(Expression<Func<TOwner, TValue>> expression, [NotNullWhen(true)] out PropertyInfo? result)
    {
        if (expression is { Body: MemberExpression { Member: PropertyInfo property } })
        {
            result = property;
            return true;
        }
        result = null;
        return false;
    }

    public static bool TryParseMethod<T>(Expression<Func<T, Action>> expression, [NotNullWhen(true)] out MethodInfo? result)
    {
        if (expression is { Body: UnaryExpression { Operand: MethodCallExpression { Object: ConstantExpression { Value: MethodInfo method } } } })
        {
            result = method;
            return true;
        }
        result = null;
        return false;
    }
}
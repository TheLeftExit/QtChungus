using System.Runtime.CompilerServices;

public abstract partial class QType
{
    public string CsName { get; }
    public string CppName { get; }
    public abstract string MarshalToCppVariant(string parameter);
    public abstract string MarshalFromCppVariant(string parameter);
    public abstract string MarshalToCsVariant(string parameter);
    public abstract string MarshalFromCsVariant(string parameter);

    protected QType(string csName, string cppName)
    {
        CsName = csName;
        CppName = cppName;
    }
}

public class QBasicType : QType
{
    public QBasicType(string csName, string cppName) : base(csName, cppName) { }

    public override string MarshalToCppVariant(string parameter) => parameter;
    public override string MarshalFromCppVariant(string parameter) => $"static_cast<{CppName}>({parameter})";
    public override string MarshalToCsVariant(string parameter) => parameter;
    public override string MarshalFromCsVariant(string parameter) => $"{parameter}.Get<{CsName}>()";
}

public class QEnumType : QType
{
    public QEnumType(string cppName, [CallerMemberName] string csName = null!) : base(csName, cppName) { }

    private static readonly QType containerType = QTypes.LongLong;

    public override string MarshalToCppVariant(string parameter) => $"static_cast<{containerType.CppName}>({parameter})";
    public override string MarshalFromCppVariant(string parameter) => $"static_cast<{CppName}>({containerType.MarshalFromCppVariant(parameter)})";
    public override string MarshalToCsVariant(string parameter) => $"Unsafe.BitCast<{CsName}, {containerType.CsName}>({parameter})";
    public override string MarshalFromCsVariant(string parameter) => $"Unsafe.BitCast<{containerType.CsName}, {CsName}>({containerType.MarshalFromCsVariant(parameter)})";
}

public partial class QObjectType : QType
{
    public static IEnumerable<QObjectType> All => all;
    public IEnumerable<QObjectType> TypeHierarchy
    {
        get
        {
            for (var x = this; x is not null; x = x.BaseType)
            {
                yield return x;
            }
        }
    }
    public static IEnumerable<QObjectType> OfType(QObjectType baseType)
    {
        return All.Where(x => x.TypeHierarchy.Contains(baseType));
    }

    private static List<QObjectType> all = new();

    public QObjectType BaseType { get; }

    public string IncludePath => CsName.StartsWith("Qc") ? $"\"{CppName}.hxx\"" : $"<{CppName}>";
    public string Namespace => CsName.StartsWith("Qc") ? "QtChungus.Internal" : "QtChungus";
    public string AccessModifier => CsName.StartsWith("Qc") ? "internal" : "public";


    public QMethod[] Methods => methods.Value;
    private Lazy<QMethod[]> methods;

    public QObjectType(
        QObjectType baseType,
        Func<QObjectType, QMethod[]>? getMethods = null,
        [CallerMemberName] string csName = null!,
        [CallerMemberName] string cppName = null!
    ) : base(csName, cppName)
    {
        all.Add(this);
        BaseType = baseType;
        getMethods ??= static x => [];
        methods = new(getMethods(this));
    }

    private static readonly QType containerType = QTypes.QObjectStar;

    public override string MarshalToCppVariant(string parameter) => parameter;
    public override string MarshalFromCppVariant(string parameter) => $"dynamic_cast<{CppName} *>({containerType.MarshalFromCppVariant(parameter)})";
    public override string MarshalToCsVariant(string parameter) => $"{parameter}.Handle";
    public override string MarshalFromCsVariant(string parameter) => $"{CsName}.FromHandle({containerType.MarshalFromCsVariant(parameter)})";
}

public class QSpecialType(
    string csName,
    string cppName,
    Func<string, string> marshalToCppVariant,
    Func<string, string> marshalFromCppVariant,
    Func<string, string> marshalToCsVariant,
    Func<string, string> marshalFromCsVariant) : QType(csName, cppName)
{
    public override string MarshalToCppVariant(string parameter) => marshalToCppVariant(parameter);
    public override string MarshalFromCppVariant(string parameter) => marshalFromCppVariant(parameter);
    public override string MarshalToCsVariant(string parameter) => marshalToCsVariant(parameter);
    public override string MarshalFromCsVariant(string parameter) => marshalFromCsVariant(parameter);
}

public class QSmuggledType(string csName, string cppName) : QType(csName, cppName)
{
    public override string MarshalToCppVariant(string parameter) => $"CVariant::FromCustomType({parameter})";
    public override string MarshalFromCppVariant(string parameter) => $"{parameter}.ToCustomType<{CppName}>()";
    public override string MarshalToCsVariant(string parameter) => $"CVariant.FromCustomType({parameter})";
    public override string MarshalFromCsVariant(string parameter) => $"{parameter}.ToCustomType<{CsName}>()";
}

public class QFunctionPointerType(string csName, string cppName) : QType(csName, cppName)
{
    public override string MarshalToCppVariant(string parameter) => throw new NotImplementedException();
    public override string MarshalFromCppVariant(string parameter) => $"reinterpret_cast<{CppName}>(static_cast<void *>({parameter}))";
    public override string MarshalToCsVariant(string parameter) => $"(nint){parameter}";
    public override string MarshalFromCsVariant(string parameter) => throw new NotImplementedException();
}

public record QMethod(QType? ReturnType, QObjectType Owner, string Name, QParameter[] Parameters)
{
    public string Id => string.Join('_', [Owner.CsName, Name, .. Parameters.Select(x => x.Type.CsName)]);
}


public record QParameter(QType Type, string Name);

public partial class QType
{
    public QParameter this[string name] => new(this, name);
}

public partial class QObjectType
{
    public QMethod Method(QType? returnType, string name, params QParameter[] parameters)
    {
        if (name is "@new") returnType = QTypes.QObjectStar;
        return new(returnType, this, name, parameters);
    }
    //public QMethod Method(string name, params QParameter[] parameters) => Method(null, name, parameters);
    public QMethod Constructor(params QParameter[] parameters) => Method(this, "@new", parameters);
    public QMethod[] Property(QType type, string getName, string? setName = null)
    {
        setName ??= $"set{char.ToUpper(getName[0])}{getName[1..]}";
        return [Method(type, getName), Method(null, setName, type["value"])];
    }
}
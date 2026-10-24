public static partial class Generator
{
    public static void GenerateCxx(string filePath)
    {
        using var writer = File.CreateText(filePath);
        writer.WriteLine(
"""
#include "CVariant.cxx"
"""
        );
        foreach (var type in QObjectType.All)
        {
            writer.WriteLine(
$$"""
#include {{type.IncludePath}}
"""
            );
        }

        writer.WriteLine(
"""
extern "C" void QcInvoke(const char *methodId, QObject *target, CVariant *returnValue, CVariant *parameters)
{
    std::string_view methodIdView{methodId};
"""
        );

        foreach (var method in QObjectType.All.SelectMany(x => x.Methods).OrderBy(x => x.Id))
        {
            var assignment = method.ReturnType is null ? "" : "*returnValue = ";
            var methodReference = method.Name is "@new" ? $"new {method.Owner.CppName}" : $"dynamic_cast<{method.Owner.CppName} *>(target)->{method.Name}";
            var methodParameters = string.Join(", ", method.Parameters.Select((x, i) => x.Type.MarshalFromCppVariant($"parameters[{i}]")));
            var methodCall = $"{methodReference}({methodParameters})";
            if (method.ReturnType is not null)
            {
                methodCall = $"*returnValue = {method.ReturnType.MarshalToCppVariant(methodCall)}";
            }
            writer.WriteLine(
$$"""
    if (methodIdView == "{{method.Id}}")
    {
        {{methodCall}};
        return;
    }
"""
            );
        }

        writer.WriteLine(
"""
    qFatal("Unknown methodId: %s, methodId");
}
"""
        );
    }
}
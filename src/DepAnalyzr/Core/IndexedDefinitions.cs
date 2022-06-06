using DepAnalyzr.Utilities;
using Mono.Cecil;

namespace DepAnalyzr.Core;

internal sealed class IndexedDefinitions
{
    private IndexedDefinitions
    (
        IReadOnlyDictionary<string, MethodDefinition> methodDefsByKey,
        IReadOnlyDictionary<string, TypeDefinition> typeDefsByKey,
        IReadOnlyDictionary<string, AssemblyDefinition> assemblyDefsByKey
    )
    {
        MethodDefsByKey = methodDefsByKey;
        TypeDefsByKey = typeDefsByKey;
        AssemblyDefsByKey = assemblyDefsByKey;
    }

    public IReadOnlyDictionary<string, MethodDefinition> MethodDefsByKey { get; }
    public IReadOnlyDictionary<string, TypeDefinition> TypeDefsByKey { get; }
    public IReadOnlyDictionary<string, AssemblyDefinition> AssemblyDefsByKey { get; }

    public static IndexedDefinitions CreateFromTypeDefinitions
    (
        IReadOnlyCollection<TypeDefinition> typeDefs
    )
    {
        var typeDefsByKey = typeDefs
            .Select(x => (key: x.Key(), value: x))
            .Where(x => NotModuleTypeDefinitionKey(x.key))
            .DistinctBy(x => x.key)
            .ToDictionary(x => x.key, x => x.value);

        var methodDefsByKey = typeDefsByKey.Values
            .Where(x => NotModuleTypeDefinitionKey(x.Key()))
            .SelectMany(x => x.Methods)
            .Select(x => (key: x.Key(), value: x))
            .ToDictionary(x => x.key, x => x.value);

        var assemblyDefsByKey = typeDefs
            .Where(x => NotModuleTypeDefinitionKey(x.Key()))
            .Select(x => x.Module.Assembly)
            .Select(x => (key: x.Key(), value: x))
            .DistinctBy(x => x.key)
            .ToDictionary(x => x.key, x => x.value);

        return new IndexedDefinitions(methodDefsByKey, typeDefsByKey, assemblyDefsByKey);
    }

    internal static bool NotModuleTypeDefinitionKey(string x) => x != "<Module>";
}
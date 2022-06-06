namespace DepAnalyzr.Core;

internal sealed class AnalysisResult
{
    public AnalysisResult
    (
        IndexedDefinitions indexedDefinitions,
        IReadOnlyDictionary<string, IReadOnlySet<string>> methodDefDependenciesByKey,
        IReadOnlyDictionary<string, IReadOnlySet<string>> typeDefDependenciesByKey,
        IReadOnlyDictionary<string, IReadOnlySet<string>> assemblyDefDependenciesByKey
    )
    {
        IndexedDefinitions = indexedDefinitions;
        MethodDefDependenciesByKey = methodDefDependenciesByKey;
        TypeDefDependenciesByKey = typeDefDependenciesByKey;
        AssemblyDefDependenciesByKey = assemblyDefDependenciesByKey;
    }

    public IndexedDefinitions IndexedDefinitions { get; }
    public IReadOnlyDictionary<string, IReadOnlySet<string>> MethodDefDependenciesByKey { get; }
    public IReadOnlyDictionary<string, IReadOnlySet<string>> TypeDefDependenciesByKey { get; }
    public IReadOnlyDictionary<string, IReadOnlySet<string>> AssemblyDefDependenciesByKey { get; }
}
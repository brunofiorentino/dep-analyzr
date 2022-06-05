namespace DepAnalyzr.Core;

public class DependencyAnalysisResult
{
    public DependencyAnalysisResult
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

    public IndexedDefinitions IndexedDefinitions { get; private set; }
    public IReadOnlyDictionary<string, IReadOnlySet<string>> MethodDefDependenciesByKey { get; private set; }
    public IReadOnlyDictionary<string, IReadOnlySet<string>> TypeDefDependenciesByKey { get; private set; }
    public IReadOnlyDictionary<string, IReadOnlySet<string>> AssemblyDefDependenciesByKey { get; private set; }
}
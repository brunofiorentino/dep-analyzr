using Mono.Cecil;

namespace DepAnalyzr.Domain.Services;

public static class MetadataLoader
{
    public static IEnumerable<AssemblyDefinition> LoadAssemblyDefinitions(IEnumerable<string> paths) =>
         paths.Select(AssemblyDefinition.ReadAssembly);
}
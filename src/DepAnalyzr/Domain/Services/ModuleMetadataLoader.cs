using Mono.Cecil;

namespace DepAnalyzr.Domain.Services;

public static class ModuleMetadataLoader
{
    public static IEnumerable<ModuleDefinition> LoadFrom(IEnumerable<string> paths) =>
        paths.Select(ModuleDefinition.ReadModule);
}
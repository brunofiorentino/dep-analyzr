using Mono.Cecil;

namespace DepAnalyzr.Core;

internal static class MonoCecilDefinitionsExtensions
{
    public static string Key(this MethodDefinition @this) => @this.ToString()!;
    public static string Key(this TypeDefinition @this) => @this.ToString()!;
    public static string Key(this AssemblyDefinition @this) => @this.ToString()!;
}
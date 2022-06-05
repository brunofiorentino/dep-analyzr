using Mono.Cecil;

namespace DepAnalyzr;

internal static class MonoCecilExtensions
{
    public static string Key(this MethodDefinition @this) => @this.ToString()!;
    public static string Key(this TypeDefinition @this) => @this.ToString()!;
    public static string Key(this AssemblyDefinition @this) => @this.ToString()!;
}
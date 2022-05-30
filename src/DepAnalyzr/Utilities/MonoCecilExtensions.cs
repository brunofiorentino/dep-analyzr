using Mono.Cecil;

namespace DepAnalyzr.Utilities;

public static class MonoCecilExtensions
{
    public static string BuildKey(this MethodDefinition @this) => @this.ToString()!;
    public static string BuildKey(this TypeDefinition @this) => @this.ToString()!;
    public static string BuildKey(this AssemblyDefinition @this) => @this.ToString()!;
}
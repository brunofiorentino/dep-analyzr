using Mono.Cecil;
using DepAnalyzr.Utilities;

namespace DepAnalyzr.Domain.Models;

public static class Analyser
{
    public static IReadOnlyDictionary<string, IReadOnlySet<MethodDefinition>> 
        AnalyseMethodDependencies(ISet<TypeDefinition> typeDefSet)
    {
        var assemblyDefSet = typeDefSet.Select(x => x.Module.Assembly.BuildKey()).ToHashSet();
        var methodDefDependenciesByMethodDef = new Dictionary<string, IReadOnlySet<MethodDefinition>>();

        foreach (var typeDef in typeDefSet)
        foreach (var methodDef in typeDef.Methods)
        {
            var dependencies = new HashSet<MethodDefinition>();

            foreach (var instruction in methodDef.Body.Instructions)
            {
                var instructionStr = instruction.ToString();
                bool IsIl(string il) => instructionStr.Contains(il);
                var isCallOrCallVirt = IsIl(": call ") || IsIl(": callvirt ") || IsIl(": newobj ");
                if (!isCallOrCallVirt) continue;

                var dependencyMethodRef = (MethodReference)instruction.Operand;
                var dependencyMethodDef = dependencyMethodRef.Resolve();

                var isDefinedInAssemblyDefSet = assemblyDefSet.Contains(dependencyMethodDef.Module.Assembly.BuildKey());
                if (!isDefinedInAssemblyDefSet) continue;

                dependencies.Add(dependencyMethodDef);
            }

            methodDefDependenciesByMethodDef[methodDef.BuildKey()] = dependencies;
        }

        return methodDefDependenciesByMethodDef;
    }
}
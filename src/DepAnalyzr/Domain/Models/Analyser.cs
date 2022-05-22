using Mono.Cecil;

namespace DepAnalyzr.Domain.Models;

public static class Analyser
{
    public static IReadOnlyDictionary<MethodDefinition, IReadOnlySet<MethodDefinition>> AnalyseMethodDependencies(
        ISet<TypeDefinition> typeDefSet)
    {
        var moduleDefSet = typeDefSet.Select(x => x.Module).ToHashSet();
        var methodDefDependenciesByMethodDef =  new Dictionary<MethodDefinition, IReadOnlySet<MethodDefinition>>();

        foreach (var typeDef in typeDefSet)
        foreach (var methodDef in typeDef.Methods)
        {
            var dependencies = new HashSet<MethodDefinition>();

            foreach (var instruction in methodDef.Body.Instructions)
            {
                var instructionStr = instruction.ToString();
                bool IsIl(string il) => instructionStr.Contains(il);
                var isCallOrCallVirt = IsIl(": call ") || IsIl(": callvirt ");
                if (!isCallOrCallVirt) continue;
                var dependencyMethodRef = (MethodReference)instruction.Operand;
                if (dependencyMethodRef is not MethodDefinition dependencyMethodDef) continue;
                
                
                // var isDefinedInModuleDefSet = moduleDefSet.Contains(dependencyMethodDef.Module);
                // if (!isDefinedInModuleDefSet) continue;
                
                dependencies.Add(dependencyMethodDef);
            }

            methodDefDependenciesByMethodDef[methodDef] = dependencies;
        }

        return methodDefDependenciesByMethodDef;
    }
}
using System.Text.RegularExpressions;
using DepAnalyzr.Core;
using DepAnalyzr.Utilities;
using Mono.Cecil;

namespace DepAnalyzr.Application;

public sealed class GenerateTypesDepMatrixCommand
{
    private readonly TextWriter _output;

    public GenerateTypesDepMatrixCommand(TextWriter output) =>
        _output = output;

    public void Execute(string assemblyPattern, string? dependentPattern , string? dependencyPattern)
    {
        var assemblyPatternRegEx = new Regex(assemblyPattern, RegexOptions.Compiled | RegexOptions.Singleline);
        var assemblyPaths = Directory
            .EnumerateFiles(Environment.CurrentDirectory, "*.dll")
            .Where(x => assemblyPatternRegEx.IsMatch(x.Split(Path.VolumeSeparatorChar.ToString()).Last()))
            .ToArray();

        var assemblyDefs = assemblyPaths.Select(AssemblyDefinition.ReadAssembly).ToArray();
        var typeDefs = assemblyDefs.Select(x => x.MainModule).SelectMany(x => x.Types).ToArray();
        var indexedDefinitions = IndexedDefinitions.CreateFromTypeDefinitions(typeDefs);
        var analysisResult = new Analyzer(indexedDefinitions).Analyze();
        var depMatrix = DependencyMatrix.CreateForTypes(analysisResult, dependentPattern, dependencyPattern);
        
        assemblyDefs.Each(x => x.Dispose());
        depMatrix.WriteTabularTo(_output);
    }
}
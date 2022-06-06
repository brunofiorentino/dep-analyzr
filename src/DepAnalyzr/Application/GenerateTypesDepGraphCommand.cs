using System.Text.RegularExpressions;
using DepAnalyzr.Core;
using DepAnalyzr.Utilities;
using Mono.Cecil;

namespace DepAnalyzr.Application;

public sealed class GenerateTypesDepGraphCommand
{
    private readonly TextWriter _output;

    public GenerateTypesDepGraphCommand(TextWriter output) =>
        _output = output;
    

    public void Execute(string assemblyPattern, string? pattern, GraphFormat format)
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
        var depGraph = DependencyGraph.CreateForTypes(analysisResult, pattern);

        assemblyDefs.Each(x => x.Dispose());
        
        var graph = format switch
        {
            GraphFormat.Svg => depGraph.ToGraphvizSvg(),
            GraphFormat.Dot => depGraph.ToGraphvizDot(),
            _ => throw new ArgumentOutOfRangeException($"Unexpected format '{format}'")
        };

        _output.WriteLine(graph);
    }
}
using System.Text.RegularExpressions;
using DepAnalyzr.Utilities;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace DepAnalyzr.Core;

internal class DependencyGraph
{
    private readonly BidirectionalGraph<string, Edge<string>> _data;

    private DependencyGraph(BidirectionalGraph<string, Edge<string>> data) =>
        _data = data;

    public string ToGraphvizDot() => _data.ToGraphviz(ga =>
    {
        ga.FormatVertex += (_, args) =>
        {
            args.VertexFormatter.Label = args.Vertex;
            args.VertexFormatter.Shape = GraphvizVertexShape.Box;
        };
    });

    public string ToGraphvizSvg()
    {
        var dotGraph = ToGraphvizDot();
        var dotGraphEscapedForBash = dotGraph
            .Replace("\"", "\\\"")
            .Replace(Environment.NewLine, string.Empty);

        const string bash = "/usr/bin/bash";
        var args = $"-c \"echo '{dotGraphEscapedForBash}' | dot -Tsvg\"";
        var (exitCode, stdOutput, stdError) = ShellExecutor.ExecuteProcessReadingStreams(bash, args);
        var succeeded = exitCode == 0;
        var svgGraph = succeeded
            ? stdOutput
            : throw new Exception($"Graph svg generation error: {stdError}");

        return svgGraph;
    }

    public static DependencyGraph CreateForAssemblies(AnalysisResult analysisResult, string? pattern)
    {
        var (friendlyDefKeys, friendlyDefDependenciesByKey) = MakeFriendly
        (
            analysisResult.IndexedDefinitions.AssemblyDefsByKey.Select(x => x.Key).ToHashSet(),
            analysisResult.AssemblyDefDependenciesByKey,
            x => analysisResult.IndexedDefinitions.AssemblyDefsByKey[x].Name.Name
        );

        return Create(friendlyDefKeys, friendlyDefDependenciesByKey, pattern);
    }

    public static DependencyGraph CreateForTypes(AnalysisResult analysisResult, string? pattern)
    {
        var (friendlyDefKeys, friendlyDefDependenciesByKey) = MakeFriendly
        (
            analysisResult.IndexedDefinitions.TypeDefsByKey.Select(x => x.Key).ToHashSet(),
            analysisResult.TypeDefDependenciesByKey,
            // TODO: Scan *nested types* in order to revert to the following lambda:
            // x => analysisResult.IndexedDefinitions.TypeDefsByKey[x].FullName
            x => x
        );

        return Create(friendlyDefKeys, friendlyDefDependenciesByKey, pattern);
    }

    private static (IReadOnlySet<string>, IReadOnlyDictionary<string, IReadOnlySet<string>>)
        MakeFriendly
        (
            IReadOnlySet<string> defKeys,
            IReadOnlyDictionary<string, IReadOnlySet<string>> defDependenciesByKey,
            Func<string, string> friendlyName
        )
    {
        var friendlyDefKeys = defKeys.Select(friendlyName).ToHashSet();
        var friendlyDefDependenciesByKey = defDependenciesByKey
            .Select(x => (
                key: friendlyName(x.Key),
                values: (IReadOnlySet<string>)x.Value.Select(friendlyName).ToHashSet()))
            .ToDictionary(x => x.key, x => x.values);

        return (friendlyDefKeys, friendlyDefDependenciesByKey);
    }

    private static DependencyGraph Create
    (
        IReadOnlySet<string> friendlyDefKeys,
        IReadOnlyDictionary<string, IReadOnlySet<string>> friendlyDefDependenciesByKey,
        string? pattern
    )
    {
        var graph = new BidirectionalGraph<string, Edge<string>>();
        var patternRegex = string.IsNullOrEmpty(pattern)
            ? null
            : new Regex(pattern, RegexOptions.Compiled | RegexOptions.Singleline);

        friendlyDefKeys
            .Where(x => patternRegex?.IsMatch(x) ?? true)
            .Each(x => graph.AddVertex(x));

        graph.Vertices
            // TODO: Scan *nested types* in order to remove the following conditional:
            // .Select(key => (dependentKey: key, dependencyKeys: friendlyDefDependenciesByKey[key]))
            .Select(key => (
                dependentKey: key,
                dependencyKeys: friendlyDefDependenciesByKey.TryGetValue(key, out var dependencyKeys)
                    ? dependencyKeys
                    : new HashSet<string>()))
            .Select(x => (
                x.dependentKey,
                dependencyKeys: x.dependencyKeys.Where(y => patternRegex?.IsMatch(y) ?? true)))
            .Where(x => x.dependencyKeys.Any())
            .SelectMany(x => x.dependencyKeys.Select(y => (x.dependentKey, dependencyKey: y)))
            // TODO: Scan *nested types* in order to remove the following conditional/filter:
            .Where(x => graph.Vertices.Contains(x.dependentKey) && graph.Vertices.Contains(x.dependencyKey))
            .Each(x => graph.AddEdge(new Edge<string>(x.dependentKey, x.dependencyKey)));


        return new DependencyGraph(graph);
    }
}
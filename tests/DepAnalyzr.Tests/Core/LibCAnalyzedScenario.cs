using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DepAnalyzr.Core;
using DepAnalyzr.Utilities;
using Mono.Cecil;
using Xunit;

namespace DepAnalyzr.Tests.Core;

// ReSharper disable once ClassNeverInstantiated.Global
public class LibCAnalyzedScenario : IAsyncLifetime
{
    private readonly CancellationTokenSource _cts;
    private readonly LibCBuiltScenario _libCBuiltScenario;
    private IReadOnlyCollection<AssemblyDefinition> _assemblyDefs = null!;

    public LibCAnalyzedScenario()
    {
        _cts = new CancellationTokenSource();
        _libCBuiltScenario = new LibCBuiltScenario(_cts);
    }

    internal AnalysisResult AnalysisResult { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _libCBuiltScenario.InitializeAsync();
        _assemblyDefs = _libCBuiltScenario.AssemblyPaths.Select(AssemblyDefinition.ReadAssembly).ToArray();
        
        var typeDefs = _assemblyDefs
            .Select(x => x.MainModule)
            .SelectMany(x => x.Types)
            .ToArray();

        var indexedDefinitions = IndexedDefinitions.CreateFromTypeDefinitions(typeDefs);
        var analyzer = new Analyzer(indexedDefinitions);

        AnalysisResult = analyzer.Analyze();
    }

    public async Task DisposeAsync()
    {
        _assemblyDefs?.Each(x => x.Dispose());
        _cts.Cancel();
        _cts.Dispose();
        await _libCBuiltScenario.DisposeAsync();
    }
}
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mono.Cecil;
using Xunit;

namespace DepAnalyzr.Tests;

public class LibCAnalysedScenario : IAsyncLifetime
{
    private readonly CancellationTokenSource _cts;
    private readonly LibCBuiltScenario _libCBuiltScenario;
    
    public LibCAnalysedScenario()
    {
        _cts = new CancellationTokenSource();
        _libCBuiltScenario = new LibCBuiltScenario(_cts);
    }
    
    public DependencyAnalysisResult AnalysisResult { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _libCBuiltScenario.InitializeAsync();
        
        var assemblyDefs = _libCBuiltScenario.AssemblyPaths.Select(AssemblyDefinition.ReadAssembly).ToArray();
        var typeDefs = assemblyDefs.Select(x => x.MainModule).SelectMany(x => x.Types).ToArray();
        var indexedDefinitions = IndexedDefinitions.From(typeDefs);
        var analyser = new Analyzer(indexedDefinitions);
        
        AnalysisResult = analyser.Analyse();
    }

    public async Task DisposeAsync()
    {
        _cts.Cancel();
        _cts.Dispose();
        await _libCBuiltScenario.DisposeAsync();
    }
}
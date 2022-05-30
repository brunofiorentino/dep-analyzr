using Mono.Cecil;
using Xunit;
using Xunit.Abstractions;

namespace DepAnalyzr.Tests.Exploratory;

public class WhenConsumingMonoCecil : IClassFixture<LibCBuiltScenario>
{
    private readonly LibCBuiltScenario _libCBuiltScenario;
    private readonly ITestOutputHelper _testOutputHelper;

    public WhenConsumingMonoCecil(LibCBuiltScenario libCBuiltScenario, ITestOutputHelper testOutputHelper)
    {
        _libCBuiltScenario = libCBuiltScenario;
        _testOutputHelper = testOutputHelper;
    }

    [Fact(Skip = "Exploratory test")]
    //[Fact]
    public void ExploreMonoCecilDefinitions()
    {
        ListMainDefinitions(_libCBuiltScenario.AssemblyADefinition.MainModule);
        ListMainDefinitions(_libCBuiltScenario.AssemblyBDefinition.MainModule);
        ListMainDefinitions(_libCBuiltScenario.AssemblyCDefinition.MainModule);
    }

    private void ListMainDefinitions(ModuleDefinition moduleDef)
    {
        _testOutputHelper.WriteLine(string.Empty);
        _testOutputHelper.WriteLine(string.Empty);
        _testOutputHelper.WriteLine($"Module '{moduleDef.Name}'");
        _testOutputHelper.WriteLine(string.Empty);

        foreach (var typeDef in moduleDef.Types)
        {
            _testOutputHelper.WriteLine(typeDef.FullName);

            foreach (var methodDef in typeDef.Methods)
            {
                _testOutputHelper.WriteLine(string.Empty);
                _testOutputHelper.WriteLine(methodDef.FullName);

                foreach (var instruction in methodDef.Body.Instructions)
                {
                    _testOutputHelper.WriteLine(instruction.ToString());
                }
            }
        }
    }
}
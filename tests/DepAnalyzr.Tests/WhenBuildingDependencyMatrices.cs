﻿using Mono.Cecil;
using Xunit;
using Xunit.Abstractions;

namespace DepAnalyzr.Tests;

public class WhenBuildingDependencyMatrices : IClassFixture<LibCBuiltScenario>
{
    private readonly LibCBuiltScenario _libCBuiltScenario;
    private readonly ITestOutputHelper _testOutputHelper;

    public WhenBuildingDependencyMatrices(LibCBuiltScenario libCBuiltScenario, ITestOutputHelper testOutputHelper)
    {
        _libCBuiltScenario = libCBuiltScenario;
        _testOutputHelper = testOutputHelper;
    }

    [Fact(Skip = "Exploratory test")]
    public void ExploreMonoCecilDefinitions()
    {
        using (var moduleADef = ModuleDefinition.ReadModule(_libCBuiltScenario.ModuleAPath))
            ListMainDefinitions(moduleADef);

        using (var moduleBDef = ModuleDefinition.ReadModule(_libCBuiltScenario.ModuleBPath))
            ListMainDefinitions(moduleBDef);

        using (var moduleCDef = ModuleDefinition.ReadModule(_libCBuiltScenario.ModuleCPath))
            ListMainDefinitions(moduleCDef);
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
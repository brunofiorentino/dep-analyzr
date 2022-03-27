using System.Collections.Generic;
using System.IO;
using System.Linq;
using DepAnalyzr.Domain.Services;
using Mono.Cecil;
using Xunit;
using Xunit.Abstractions;

namespace DepAnalyzr.Tests;

public class WhenLoadingMetadata : IClassFixture<LibCBuiltScenario>
{
    private readonly LibCBuiltScenario _libCBuiltScenario;
    private readonly ITestOutputHelper _testOutputHelper;

    public WhenLoadingMetadata(LibCBuiltScenario libCBuiltScenario, ITestOutputHelper testOutputHelper)
    {
        _libCBuiltScenario = libCBuiltScenario;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void ModuleDefinitionsAreLoadedFromPhysicalPaths()
    {
        var moduleDefinitions = new List<ModuleDefinition>();

        try
        {
            // Adds individually to allow disposal in case of failure of some item.
            foreach (var moduleDefinition in ModuleMetadataLoader.LoadFrom(_libCBuiltScenario.ModulePaths))
                moduleDefinitions.Add(moduleDefinition);

            Assert.Equal(3, moduleDefinitions.Count);
        }
        finally
        {
            if (moduleDefinitions.Any())
                foreach (var moduleDefinition in moduleDefinitions)
                    moduleDefinition.Dispose();
        }
    }
}
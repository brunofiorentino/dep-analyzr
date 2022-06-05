using System;
using System.Linq;
using DepAnalyzr.Utilities;
using Xunit;
using Xunit.Abstractions;

namespace DepAnalyzr.Tests.Core;

[Collection(nameof(LibCAnalysedCollection))]
public class WhenAnalysingDependencies // : IClassFixture<LibCAnalysedScenario>
{
    private readonly LibCAnalysedScenario _libCAnalysedScenario;
    private readonly ITestOutputHelper _testOutputHelper;

    public WhenAnalysingDependencies
    (
        LibCAnalysedScenario libCAnalysedScenario,
        ITestOutputHelper testOutputHelper
    )
    {
        _libCAnalysedScenario = libCAnalysedScenario;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void ExpectedMethodDependenciesAreDetected()
    {
        var methodDependencies = _libCAnalysedScenario
            .AnalysisResult
            .MethodDefDependenciesByKey["System.Void DepAnalyzr.Tests.LibC.LibCType01::DoSomething()"];

        Assert.Equal(7, methodDependencies.Count);

        Assert.Contains(methodDependencies,
            x => x == "System.Double DepAnalyzr.Tests.LibA.LibAType01::StaticDoSomething()");

        Assert.Contains(methodDependencies,
            x => x == "System.Int32 DepAnalyzr.Tests.LibA.LibAType01::get_StaticSomeProp()");

        Assert.Contains(methodDependencies, x => x == "System.Void DepAnalyzr.Tests.LibA.LibAType01::.ctor()");
        Assert.Contains(methodDependencies, x => x == "System.Void DepAnalyzr.Tests.LibA.LibAType01::DoSomething()");
        Assert.Contains(methodDependencies, x => x == "System.Int32 DepAnalyzr.Tests.LibA.LibAType01::get_SomeProp()");
        Assert.Contains(methodDependencies, x => x == "System.Void DepAnalyzr.Tests.LibB.LibBType01::.ctor()");
        Assert.Contains(methodDependencies, x => x == "System.Void DepAnalyzr.Tests.LibB.LibBType01::DoSomething()");
    }

    [Fact]
    public void ExpectedTypeDependenciesAreDetected()
    {
        var typeDependencies = _libCAnalysedScenario
            .AnalysisResult
            .TypeDefDependenciesByKey["DepAnalyzr.Tests.LibC.LibCType01"];

        Assert.Equal(2, typeDependencies.Count);
        Assert.Contains(typeDependencies, x => x == "DepAnalyzr.Tests.LibA.LibAType01");
        Assert.Contains(typeDependencies, x => x == "DepAnalyzr.Tests.LibB.LibBType01");
    }

    [Fact]
    public void ExpectedAssemblyDependenciesAreDetected()
    {
        var assemblyDependencies = _libCAnalysedScenario
            .AnalysisResult
            .AssemblyDefDependenciesByKey[
                "DepAnalyzr.Tests.LibC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"];

        Assert.Equal(2, assemblyDependencies.Count);
        Assert.Contains(assemblyDependencies,
            x => x == "DepAnalyzr.Tests.LibA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

        Assert.Contains(assemblyDependencies,
            x => x == "DepAnalyzr.Tests.LibB, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
    }

    [Fact(Skip = "Exploratory")]
    public void PrintDependencies()
    {
        var analysisResult = _libCAnalysedScenario.AnalysisResult;

        foreach (var assemblyDefKey in analysisResult.AssemblyDefDependenciesByKey.Keys.OrderBy(x => x))
        {
            var assemblyDef = analysisResult.IndexedDefinitions.AssemblyDefsByKey[assemblyDefKey];
            _testOutputHelper.WriteLine(new string('=', 80));
            _testOutputHelper.WriteLine(assemblyDef.Key());

            foreach (var typeDef in assemblyDef.MainModule.Types.OrderBy(x => x.FullName))
            {
                _testOutputHelper.WriteLine(new string('-', 80));
                _testOutputHelper.WriteLine(typeDef.Key());

                foreach (var methodDef in typeDef.Methods.OrderBy(x => x.FullName))
                {
                    _testOutputHelper.WriteLine(new string('.', 80));
                    _testOutputHelper.WriteLine(methodDef.Key());
                    var methodDependencies = analysisResult.MethodDefDependenciesByKey[methodDef.Key()];
                    _testOutputHelper.WriteLine($"{nameof(methodDependencies)}: {methodDependencies.Count}");

                    foreach (var methodDependency in methodDependencies)
                        _testOutputHelper.WriteLine(methodDependency);
                }
            }

            _testOutputHelper.WriteLine(Environment.NewLine);
            _testOutputHelper.WriteLine(Environment.NewLine);
        }
    }
}
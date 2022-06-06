using Xunit;
using Xunit.Abstractions;

namespace DepAnalyzr.Tests.Core;

[Collection(nameof(LibCAnalyzedCollection))]
public class WhenAnalyzingDependencies 
{
    private readonly LibCAnalyzedScenario _libCAnalyzedScenario;
    private readonly ITestOutputHelper _testOutputHelper;

    public WhenAnalyzingDependencies(LibCAnalyzedScenario libCAnalyzedScenario, ITestOutputHelper testOutputHelper)
    {
        _libCAnalyzedScenario = libCAnalyzedScenario;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void MethodDependenciesAreDetected()
    {
        var methodDependencies = _libCAnalyzedScenario
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
    public void TypeDependenciesAreDetected()
    {
        var typeDependencies = _libCAnalyzedScenario
            .AnalysisResult
            .TypeDefDependenciesByKey["DepAnalyzr.Tests.LibC.LibCType01"];

        Assert.Equal(2, typeDependencies.Count);
        Assert.Contains(typeDependencies, x => x == "DepAnalyzr.Tests.LibA.LibAType01");
        Assert.Contains(typeDependencies, x => x == "DepAnalyzr.Tests.LibB.LibBType01");
    }

    [Fact]
    public void AssemblyDependenciesAreDetected()
    {
        var assemblyDependencies = _libCAnalyzedScenario
            .AnalysisResult
            .AssemblyDefDependenciesByKey[
                "DepAnalyzr.Tests.LibC, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"];

        Assert.Equal(2, assemblyDependencies.Count);
        Assert.Contains(assemblyDependencies,
            x => x == "DepAnalyzr.Tests.LibA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

        Assert.Contains(assemblyDependencies,
            x => x == "DepAnalyzr.Tests.LibB, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
    }
}
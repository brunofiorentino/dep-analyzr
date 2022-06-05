using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Xunit;
using Xunit.Abstractions;
using static DepAnalyzr.Tests.TestNames;

namespace DepAnalyzr.Tests;

public class WhenCreatingTypeDependencyMatrices : IClassFixture<LibCAnalysedScenario>
{
    private readonly LibCAnalysedScenario _libCAnalysedScenario;
    private readonly ITestOutputHelper _testOutputHelper;

    public WhenCreatingTypeDependencyMatrices
    (
        LibCAnalysedScenario libCAnalysedScenario,
        ITestOutputHelper testOutputHelper
    )
    {
        _libCAnalysedScenario = libCAnalysedScenario;
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void ExpectedDependenciesAreDetected()
    {
        var analysisResult = _libCAnalysedScenario.AnalysisResult;
        var depMatrix = DependencyMatrixView.CreateForTypes(analysisResult);
        var defsByKey = analysisResult.IndexedDefinitions.TypeDefsByKey;

        AssertExpectedDepMatrixLengths(defsByKey.Keys.Count() + 1, depMatrix);
        AssertExpectedLabelNames(depMatrix, defsByKey);
        AssertCellsProperlyPointDependencies(depMatrix);
    }

    private static void AssertExpectedDepMatrixLengths(int length, string[,] depMatrix)
    {
        Assert.Equal(length, depMatrix.GetLength(0));
        Assert.Equal(length, depMatrix.GetLength(1));
    }

    private static void AssertExpectedLabelNames
    (
        string[,] depMatrix,
        IReadOnlyDictionary<string, TypeDefinition> defsByKey
    )
    {
        Assert.Null(depMatrix[0, 0]);

        // var libAAssemblyFriendlyName = defsByKey[LibAAssemblyName].FullName;
        // var libBAssemblyFriendlyName = defsByKey[LibBAssemblyName].FullName;
        // var libCAssemblyFriendlyName = defsByKey[LibCAssemblyName].FullName;
        //
        // Assert.Equal(libAAssemblyFriendlyName, depMatrix[0, 1]);
        // Assert.Equal(libBAssemblyFriendlyName, depMatrix[0, 2]);
        // Assert.Equal(libCAssemblyFriendlyName, depMatrix[0, 3]);
        //
        // Assert.Equal(libAAssemblyFriendlyName, depMatrix[1, 0]);
        // Assert.Equal(libBAssemblyFriendlyName, depMatrix[2, 0]);
        // Assert.Equal(libCAssemblyFriendlyName, depMatrix[3, 0]);
    }

    private static void AssertCellsProperlyPointDependencies(string[,] depMatrix)
    {
        Assert.Equal(n, depMatrix[1, 1]);
        // Assert.Equal(n, depMatrix[1, 2]);
        // Assert.Equal(n, depMatrix[1, 3]);
        //
        // Assert.Equal(y, depMatrix[2, 1]);
        // Assert.Equal(n, depMatrix[2, 2]);
        // Assert.Equal(n, depMatrix[2, 3]);
        //
        // Assert.Equal(y, depMatrix[3, 1]);
        // Assert.Equal(y, depMatrix[3, 2]);
        // Assert.Equal(n, depMatrix[3, 3]);
    }
}
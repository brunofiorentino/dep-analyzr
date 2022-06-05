using System.Linq;
using DepAnalyzr.Core;
using Xunit;
using Xunit.Abstractions;
using static DepAnalyzr.Tests.Core.HandyNames;

namespace DepAnalyzr.Tests.Core;

[Collection(nameof(LibCAnalysedCollection))]
public class WhenCreatingTypeDependencyMatrices // : IClassFixture<LibCAnalysedScenario>
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
        var depMatrix = DependencyMatrix.CreateForTypes(analysisResult);
        var defsByKey = analysisResult.IndexedDefinitions.TypeDefsByKey;

        AssertExpectedDepMatrixLengths(defsByKey.Keys.Count() + 1, depMatrix.Data);
        AssertExpectedLabelNames(depMatrix.Data);
        AssertCellsProperlyPointDependencies(depMatrix.Data);

        using var testTextWriterOutput = new TestTextWriter(_testOutputHelper);
        depMatrix.WriteTabularTo(testTextWriterOutput);
    }

    private static void AssertExpectedDepMatrixLengths(int length, string[,] depMatrixData)
    {
        Assert.Equal(length, depMatrixData.GetLength(0));
        Assert.Equal(length, depMatrixData.GetLength(1));
    }

    private static void AssertExpectedLabelNames(string[,] depMatrixData)
    {
        Assert.Null(depMatrixData[0, 0]);

        Assert.Equal(LibAType01Name, depMatrixData[0, 1]);
        Assert.Equal(LibBType01Name, depMatrixData[0, 2]);
        Assert.Equal(LibCType01Name, depMatrixData[0, 3]);
        
        Assert.Equal(LibAType01Name, depMatrixData[1, 0]);
        Assert.Equal(LibBType01Name, depMatrixData[2, 0]);
        Assert.Equal(LibCType01Name, depMatrixData[3, 0]);
    }

    private static void AssertCellsProperlyPointDependencies(string[,] depMatrixData)
    {
        Assert.Equal(No, depMatrixData[1, 1]);
        Assert.Equal(No, depMatrixData[1, 2]);
        Assert.Equal(No, depMatrixData[1, 3]);

        Assert.Equal(Yes, depMatrixData[2, 1]);
        Assert.Equal(No, depMatrixData[2, 2]);
        Assert.Equal(No, depMatrixData[2, 3]);

        Assert.Equal(Yes, depMatrixData[3, 1]);
        Assert.Equal(Yes, depMatrixData[3, 2]);
        Assert.Equal(No, depMatrixData[3, 3]);
    }
}
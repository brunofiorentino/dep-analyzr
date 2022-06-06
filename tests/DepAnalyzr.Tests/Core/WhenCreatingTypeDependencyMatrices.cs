using System.Linq;
using DepAnalyzr.Core;
using DepAnalyzr.Tests.TestUtilities;
using Xunit;
using Xunit.Abstractions;
using static DepAnalyzr.Tests.Core.HandyNames;

namespace DepAnalyzr.Tests.Core;

[Collection(nameof(LibCAnalyzedCollection))]
public class WhenCreatingTypeDependencyMatrices
{
    private readonly LibCAnalyzedScenario _libCAnalyzedScenario;
    private readonly ITestOutputHelper _output;

    public WhenCreatingTypeDependencyMatrices(LibCAnalyzedScenario libCAnalyzedScenario, ITestOutputHelper output)
    {
        _libCAnalyzedScenario = libCAnalyzedScenario;
        _output = output;
    }

    [Fact]
    public void NonFilteredRelationshipsAreDetected()
    {
        var analysisResult = _libCAnalyzedScenario.AnalysisResult;
        var depMatrix = DependencyMatrix.CreateForTypes(analysisResult, null, null);
        var defsByKey = analysisResult.IndexedDefinitions.TypeDefsByKey;

        AssertExpectedDepMatrixLengths(defsByKey.Keys.Count() + 1, depMatrix.Data);
        AssertExpectedNonFilteredLabelNames(depMatrix.Data);
        AssertExpectedNonFilteredDependenciesArePointed(depMatrix.Data);

        using var testTextWriter = new TestTextWriter(_output);
        depMatrix.WriteTabularTo(testTextWriter);
    }

    [Fact]
    public void FilteredRelationshipsAreDetected()
    {
        var analysisResult = _libCAnalyzedScenario.AnalysisResult;
        const string dependentAndDependencyPattern = "(DepAnalyzr.Tests.LibA|DepAnalyzr.Tests.LibB)";
        var depMatrix = DependencyMatrix.CreateForTypes(
            analysisResult, dependentAndDependencyPattern, dependentAndDependencyPattern);
        
        var defsByKey = analysisResult.IndexedDefinitions.TypeDefsByKey;

        AssertExpectedDepMatrixLengths(3, depMatrix.Data);
        AssertExpectedFilteredLabelNames(depMatrix.Data);
        AssertExpectedFilteredDependenciesArePointed(depMatrix.Data);

        using var testTextWriter = new TestTextWriter(_output);
        depMatrix.WriteTabularTo(testTextWriter);
    }

    private static void AssertExpectedDepMatrixLengths(int length, string[,] depMatrixData)
    {
        Assert.Equal(length, depMatrixData.GetLength(0));
        Assert.Equal(length, depMatrixData.GetLength(1));
    }

    private static void AssertExpectedNonFilteredLabelNames(string[,] depMatrixData)
    {
        Assert.Null(depMatrixData[0, 0]);

        Assert.Equal(LibAType01Name, depMatrixData[0, 1]);
        Assert.Equal(LibBType01Name, depMatrixData[0, 2]);
        Assert.Equal(LibCType01Name, depMatrixData[0, 3]);
        
        Assert.Equal(LibAType01Name, depMatrixData[1, 0]);
        Assert.Equal(LibBType01Name, depMatrixData[2, 0]);
        Assert.Equal(LibCType01Name, depMatrixData[3, 0]);
    }

    private static void AssertExpectedFilteredLabelNames(string[,] depMatrixData)
    {
        Assert.Null(depMatrixData[0, 0]);

        Assert.Equal(LibAType01Name, depMatrixData[0, 1]);
        Assert.Equal(LibBType01Name, depMatrixData[0, 2]);
        
        Assert.Equal(LibAType01Name, depMatrixData[1, 0]);
        Assert.Equal(LibBType01Name, depMatrixData[2, 0]);
    }

    private static void AssertExpectedNonFilteredDependenciesArePointed(string[,] depMatrixData)
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

    private static void AssertExpectedFilteredDependenciesArePointed(string[,] depMatrixData)
    {
        Assert.Equal(No, depMatrixData[1, 1]);
        Assert.Equal(No, depMatrixData[1, 2]);

        Assert.Equal(Yes, depMatrixData[2, 1]);
        Assert.Equal(No, depMatrixData[2, 2]);
    }
}
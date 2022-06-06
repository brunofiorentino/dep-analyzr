using System.Linq;
using DepAnalyzr.Core;
using DepAnalyzr.Tests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace DepAnalyzr.Tests.Core;

[Collection(nameof(LibCAnalyzedCollection))]
public class WhenCreatingMethodDependencyMatrices
{
    private readonly LibCAnalyzedScenario _libCAnalyzedScenario;
    private readonly ITestOutputHelper _output;

    public WhenCreatingMethodDependencyMatrices
    (
        LibCAnalyzedScenario libCAnalyzedScenario,
        ITestOutputHelper output
    )
    {
        _libCAnalyzedScenario = libCAnalyzedScenario;
        _output = output;
    }

    [Fact]
    public void RelationshipsAreDetected()
    {
        var analysisResult = _libCAnalyzedScenario.AnalysisResult;
        var depMatrix = DependencyMatrix.CreateForMethods(analysisResult, null, null);
        var defsByKey = analysisResult.IndexedDefinitions.MethodDefsByKey;

        AssertExpectedDepMatrixLengths(defsByKey.Keys.Count() + 1, depMatrix.Data);
        // TODO: AssertExpectedLabelNames(depMatrix.Data);
        // TODO: AssertCellsProperlyPointDependencies(depMatrix.Data);

        using var testTextWriter = new TestTextWriter(_output);
        depMatrix.WriteTabularTo(testTextWriter);
    }

    private static void AssertExpectedDepMatrixLengths(int length, string[,] depMatrixData)
    {
        Assert.Equal(length, depMatrixData.GetLength(0));
        Assert.Equal(length, depMatrixData.GetLength(1));
    }
}
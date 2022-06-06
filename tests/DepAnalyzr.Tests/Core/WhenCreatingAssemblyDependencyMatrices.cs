using System.Collections.Generic;
using System.Linq;
using DepAnalyzr.Core;
using DepAnalyzr.Tests.TestUtilities;
using Mono.Cecil;
using Xunit;
using Xunit.Abstractions;
using static DepAnalyzr.Tests.Core.HandyNames;

namespace DepAnalyzr.Tests.Core;

[Collection(nameof(LibCAnalyzedCollection))]
public class WhenCreatingAssemblyDependencyMatrices
{
    private readonly LibCAnalyzedScenario _libCAnalyzedScenario;
    private readonly ITestOutputHelper _output;

    public WhenCreatingAssemblyDependencyMatrices(LibCAnalyzedScenario libCAnalyzedScenario, ITestOutputHelper output)
    {
        _libCAnalyzedScenario = libCAnalyzedScenario;
        _output = output;
    }

    // Note: dotnet test --logger "console;verbosity=detailed"

    [Fact]
    public void NonFilteredRelationshipsAreDetected()
    {
        var analysisResult = _libCAnalyzedScenario.AnalysisResult;
        var depMatrix = DependencyMatrix.CreateForAssemblies(analysisResult, null, null);
        var defsByKey = analysisResult.IndexedDefinitions.AssemblyDefsByKey;

        AssertExpectedDepMatrixLengths(defsByKey.Keys.Count() + 1, depMatrix.Data);
        AssertExpectedNonFilteredLabelNames(depMatrix.Data, defsByKey);
        AssertExpectedNonFilteredDependenciesArePointed(depMatrix.Data);

        using var testTextWriter = new TestTextWriter(_output);
        depMatrix.WriteTabularTo(testTextWriter);
    }

    [Fact]
    public void FilteredRelationshipsAreDetected()
    {
        var analysisResult = _libCAnalyzedScenario.AnalysisResult;
        const string dependentAndDependencyPattern = "(DepAnalyzr.Tests.LibA|DepAnalyzr.Tests.LibB)";
        var depMatrix = DependencyMatrix.CreateForAssemblies(
            analysisResult, dependentAndDependencyPattern, dependentAndDependencyPattern);

        var defsByKey = analysisResult.IndexedDefinitions.AssemblyDefsByKey;

        AssertExpectedDepMatrixLengths(3, depMatrix.Data);
        AssertExpectedFilteredLabelNames(depMatrix.Data, defsByKey);
        AssertExpectedFilteredDependenciesArePointed(depMatrix.Data);

        using var testTextWriter = new TestTextWriter(_output);
        depMatrix.WriteTabularTo(testTextWriter);
    }

    private static void AssertExpectedDepMatrixLengths(int length, string[,] depMatrix)
    {
        Assert.Equal(length, depMatrix.GetLength(0));
        Assert.Equal(length, depMatrix.GetLength(1));
    }

    private static void AssertExpectedNonFilteredLabelNames
    (
        string[,] depMatrix,
        IReadOnlyDictionary<string, AssemblyDefinition> defsByKey
    )
    {
        Assert.Null(depMatrix[0, 0]);

        var libAAssemblyFriendlyName = defsByKey[LibAAssemblyName].Name.Name;
        var libBAssemblyFriendlyName = defsByKey[LibBAssemblyName].Name.Name;
        var libCAssemblyFriendlyName = defsByKey[LibCAssemblyName].Name.Name;

        Assert.Equal(libAAssemblyFriendlyName, depMatrix[0, 1]);
        Assert.Equal(libBAssemblyFriendlyName, depMatrix[0, 2]);
        Assert.Equal(libCAssemblyFriendlyName, depMatrix[0, 3]);

        Assert.Equal(libAAssemblyFriendlyName, depMatrix[1, 0]);
        Assert.Equal(libBAssemblyFriendlyName, depMatrix[2, 0]);
        Assert.Equal(libCAssemblyFriendlyName, depMatrix[3, 0]);
    }

    private static void AssertExpectedFilteredLabelNames
    (
        string[,] depMatrix,
        IReadOnlyDictionary<string, AssemblyDefinition> defsByKey
    )
    {
        Assert.Null(depMatrix[0, 0]);

        var libAAssemblyFriendlyName = defsByKey[LibAAssemblyName].Name.Name;
        var libBAssemblyFriendlyName = defsByKey[LibBAssemblyName].Name.Name;

        Assert.Equal(libAAssemblyFriendlyName, depMatrix[0, 1]);
        Assert.Equal(libBAssemblyFriendlyName, depMatrix[0, 2]);

        Assert.Equal(libAAssemblyFriendlyName, depMatrix[1, 0]);
        Assert.Equal(libBAssemblyFriendlyName, depMatrix[2, 0]);
    }

    private static void AssertExpectedNonFilteredDependenciesArePointed(string[,] depMatrix)
    {
        Assert.Equal(No, depMatrix[1, 1]);
        Assert.Equal(No, depMatrix[1, 2]);
        Assert.Equal(No, depMatrix[1, 3]);

        Assert.Equal(Yes, depMatrix[2, 1]);
        Assert.Equal(No, depMatrix[2, 2]);
        Assert.Equal(No, depMatrix[2, 3]);

        Assert.Equal(Yes, depMatrix[3, 1]);
        Assert.Equal(Yes, depMatrix[3, 2]);
        Assert.Equal(No, depMatrix[3, 3]);
    }
    
    private static void AssertExpectedFilteredDependenciesArePointed(string[,] depMatrix)
    {
        Assert.Equal(No, depMatrix[1, 1]);
        Assert.Equal(No, depMatrix[1, 2]);

        Assert.Equal(Yes, depMatrix[2, 1]);
        Assert.Equal(No, depMatrix[2, 2]);
    }
}
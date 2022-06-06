using System.IO;
using System.Linq;
using System.Text;
using DepAnalyzr.Application;
using DepAnalyzr.Core;
using DepAnalyzr.Tests.Core;
using DepAnalyzr.Tests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace DepAnalyzr.Tests.Application;

[Collection(nameof(CommandsCollection))]
public class WhenGeneratingTypesDepMatrices : IClassFixture<LibCBuiltScenario>
{
    private readonly LibCBuiltScenario _libCBuiltScenario;
    private readonly ITestOutputHelper _output;

    public WhenGeneratingTypesDepMatrices(LibCBuiltScenario libCBuiltScenario, ITestOutputHelper output)
    {
        _libCBuiltScenario = libCBuiltScenario;
        _output = output;
    }

    [Fact]
    public void TableIsRenderedForMatchedDependentPattern()
    {
        var dependentPattern = HandyNames.LibAType01Name.Split(".").Last();
        var assertionOutput = GivenCommandRan(dependentPattern, null);

        Assert.Contains(DependencyMatrix.FirstTableCellLabel, assertionOutput);
        Assert.Contains(HandyNames.LibAType01Name, assertionOutput);
        Assert.Contains("Count: 1", assertionOutput);
    }

    [Fact]
    public void TableIsRenderedForMatchedDependencyPattern()
    {
        const string dependentAndDependencyPattern = "LibC";
        var assertionOutput = GivenCommandRan(dependentAndDependencyPattern,dependentAndDependencyPattern);

        Assert.Contains(DependencyMatrix.FirstTableCellLabel, assertionOutput);
        Assert.DoesNotContain("LibA", assertionOutput);
        Assert.DoesNotContain("LibB", assertionOutput);
        Assert.Contains("Count: 1", assertionOutput);
    }

    [Fact]
    public void TableIsRenderedForUnmatchedDependentPattern()
    {
        const string dependentPattern = "WillNotMatchAnyType";
        var assertionOutput = GivenCommandRan(dependentPattern, null);

        Assert.Contains(DependencyMatrix.FirstTableCellLabel, assertionOutput);
        Assert.Contains("Count: 0", assertionOutput);
    }

    private string GivenCommandRan(string? dependentPattern, string? dependencyPattern)
    {
        var assertionOutputBuilder = new StringBuilder();

        using (TextWriter assertionOutput = new StringWriter(assertionOutputBuilder))
        using (TextWriter testOutput = new TestTextWriter(_output))
        using (var commandOutput = new CompositeTextWriter(new[] { assertionOutput, testOutput }))
            new GenerateTypesDepMatrixCommand(commandOutput)
                .Execute("^DepAnalyzr.Tests.Lib", dependentPattern, dependencyPattern);

        return assertionOutputBuilder.ToString();
    }
}
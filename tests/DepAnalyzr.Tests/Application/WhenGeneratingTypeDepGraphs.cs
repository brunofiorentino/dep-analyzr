using System.IO;
using System.Text;
using DepAnalyzr.Application;
using DepAnalyzr.Tests.Core;
using DepAnalyzr.Tests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace DepAnalyzr.Tests.Application;

[Collection(nameof(CommandsCollection))]
public class WhenGeneratingTypeDepGraphs : IClassFixture<LibCBuiltScenario>
{
    private readonly ITestOutputHelper _output;

    public WhenGeneratingTypeDepGraphs(ITestOutputHelper output) =>
        _output = output;
    

    [Theory]
    [InlineData(GraphFormat.Svg)]
    [InlineData(GraphFormat.Dot)]
    public void GraphIsRenderedForMatchedPattern(GraphFormat format)
    {
        const string dependentPattern = $"({HandyNames.LibAType01Name}|{HandyNames.LibBType01Name})";
        var assertionOutput = GivenCommandRan(dependentPattern, format);
        var graphFormatHint = format == GraphFormat.Svg 
            ? HandyNames.GraphvizSvgFormatHint : HandyNames.GraphvizDotFormatHint;

        Assert.Contains(graphFormatHint, assertionOutput);
        Assert.Contains(HandyNames.LibAType01Name, assertionOutput);
        Assert.Contains(HandyNames.LibBType01Name, assertionOutput);
        Assert.DoesNotContain(HandyNames.LibCType01Name, assertionOutput);
    }

    private string GivenCommandRan(string? pattern, GraphFormat format)
    {
        var assertionOutputBuilder = new StringBuilder();

        using (TextWriter assertionOutput = new StringWriter(assertionOutputBuilder))
        using (TextWriter testOutput = new TestTextWriter(_output))
        using (var commandOutput = new CompositeTextWriter(new[] { assertionOutput, testOutput }))
            new GenerateTypesDepGraphCommand(commandOutput)
                .Execute("^DepAnalyzr.Tests.Lib", pattern, format);

        return assertionOutputBuilder.ToString();
    }
}
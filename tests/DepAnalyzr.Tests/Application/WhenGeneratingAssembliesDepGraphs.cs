using System.IO;
using System.Linq;
using System.Text;
using DepAnalyzr.Application;
using DepAnalyzr.Tests.Core;
using DepAnalyzr.Tests.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace DepAnalyzr.Tests.Application;

[Collection(nameof(CommandsCollection))]
public class WhenGeneratingAssembliesDepGraphs : IClassFixture<LibCBuiltScenario>
{
    private readonly ITestOutputHelper _output;

    public WhenGeneratingAssembliesDepGraphs(ITestOutputHelper output) =>
        _output = output;
    

    [Theory]
    [InlineData(GraphFormat.Svg)]
    [InlineData(GraphFormat.Dot)]
    public void GraphIsRenderedForMatchedPattern(GraphFormat format)
    {
        string FriendlyAssemblyName(string x) => x.Split(",").First();
        
        var libAAssemblyName = FriendlyAssemblyName(HandyNames.LibAAssemblyName);
        var libBAssemblyName = FriendlyAssemblyName(HandyNames.LibBAssemblyName);
        var libCAssemblyName = FriendlyAssemblyName(HandyNames.LibCAssemblyName);
        var dependentPattern = $"({libAAssemblyName}|{libBAssemblyName})";
        var assertionOutput = GivenCommandRan(dependentPattern, format);
        var graphFormatHint = format == GraphFormat.Svg 
            ? HandyNames.GraphvizSvgFormatHint : HandyNames.GraphvizDotFormatHint;

        Assert.Contains(graphFormatHint, assertionOutput);
        Assert.Contains(libAAssemblyName, assertionOutput);
        Assert.Contains(libBAssemblyName, assertionOutput);
        Assert.DoesNotContain(libCAssemblyName, assertionOutput);
    }

    private string GivenCommandRan(string? pattern, GraphFormat format)
    {
        var assertionOutputBuilder = new StringBuilder();

        using (TextWriter assertionOutput = new StringWriter(assertionOutputBuilder))
        using (TextWriter testOutput = new TestTextWriter(_output))
        using (var commandOutput = new CompositeTextWriter(new[] { assertionOutput, testOutput }))
            new GenerateAssembliesDepGraphCommand(commandOutput)
                .Execute("^DepAnalyzr.Tests.Lib", pattern, format);

        return assertionOutputBuilder.ToString();
    }
}
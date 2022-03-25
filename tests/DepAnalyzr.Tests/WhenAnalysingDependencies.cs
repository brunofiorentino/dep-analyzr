using Xunit;

namespace DepAnalyzr.Tests;

public class WhenAnalysingDependencies : IClassFixture<LibCBuiltScenario>
{
    private readonly LibCBuiltScenario _libCBuiltScenario;

    public WhenAnalysingDependencies(LibCBuiltScenario libCBuiltScenario)
    {
        _libCBuiltScenario = libCBuiltScenario;
    }

    [Fact]
    public void LibCBuiltScenarioBuiltOutputPathIsProvided()
    {
        Assert.NotNull(_libCBuiltScenario.TargetLibrariesBuildOutputPath);
    }
}
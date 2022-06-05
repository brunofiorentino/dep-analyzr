using System.IO;
using System.Text;
using Xunit.Abstractions;

namespace DepAnalyzr.Tests.TestUtilities;

public class TestTextWriter : TextWriter
{
    private readonly ITestOutputHelper _testOutput;

    public TestTextWriter(ITestOutputHelper testOutput) =>
        _testOutput = testOutput;

    public override Encoding Encoding { get; } = Encoding.UTF8;

    public override void WriteLine(string? value) =>
        _testOutput.WriteLine(value);
}
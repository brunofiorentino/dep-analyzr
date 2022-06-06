using System.Collections.Generic;
using System.IO;
using System.Text;
using DepAnalyzr.Utilities;

namespace DepAnalyzr.Tests.TestUtilities;

public class CompositeTextWriter : TextWriter
{
    private readonly IReadOnlyCollection<TextWriter> _targets;

    public CompositeTextWriter(IReadOnlyCollection<TextWriter> targets) =>
        _targets = targets;


    public override Encoding Encoding { get; } = Encoding.UTF8;

    public override void WriteLine(string? value) =>
        _targets.Each(x => x.WriteLine(value));
}
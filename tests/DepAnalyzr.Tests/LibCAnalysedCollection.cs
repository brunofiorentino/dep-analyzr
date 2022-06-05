using Xunit;

namespace DepAnalyzr.Tests;

[CollectionDefinition(nameof(LibCAnalysedCollection))]
public class LibCAnalysedCollection : ICollectionFixture<LibCAnalysedScenario>
{
}
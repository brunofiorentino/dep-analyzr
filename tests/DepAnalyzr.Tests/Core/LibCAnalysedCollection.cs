using Xunit;

namespace DepAnalyzr.Tests.Core;

[CollectionDefinition(nameof(LibCAnalysedCollection))]
public class LibCAnalysedCollection : ICollectionFixture<LibCAnalysedScenario>
{
}
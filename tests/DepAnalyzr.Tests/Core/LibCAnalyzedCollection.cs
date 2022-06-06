using Xunit;

namespace DepAnalyzr.Tests.Core;

[CollectionDefinition(nameof(LibCAnalyzedCollection))]
public class LibCAnalyzedCollection : ICollectionFixture<LibCAnalyzedScenario>
{
}
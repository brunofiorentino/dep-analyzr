using Xunit;

namespace DepAnalyzr.Tests.Application;

// Need to stop sharing analyzed assemblies built output in order to re-enabling parallelization. 
[CollectionDefinition(nameof(CommandsCollection), DisableParallelization = true)]
public class CommandsCollection
{
    
}
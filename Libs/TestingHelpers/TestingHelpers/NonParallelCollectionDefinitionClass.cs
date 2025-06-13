using Xunit;

namespace TestingHelpers;

[CollectionDefinition("Non-Parallel Collection", DisableParallelization = true)]
public class NonParallelCollectionDefinitionClass
{
}

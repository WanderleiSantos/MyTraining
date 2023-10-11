using IntegrationTests.V1;
using Xunit;

namespace IntegrationTests;

[CollectionDefinition("mytraining collection")]
public class SharedTestCollection : ICollectionFixture<DatabaseFixture>
{
    
}
using IntegrationTests.V1;
using Xunit;

namespace IntegrationTests;

[CollectionDefinition("mytraining collection")]
public class SharedTestColletion : ICollectionFixture<DatabaseFixture>
{
    
}
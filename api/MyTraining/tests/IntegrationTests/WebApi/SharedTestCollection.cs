using Xunit;

namespace IntegrationTests.WebApi;

[CollectionDefinition("mytraining collection")]
public class SharedTestCollection : ICollectionFixture<MyTrainingApiFactory>
{
    
}
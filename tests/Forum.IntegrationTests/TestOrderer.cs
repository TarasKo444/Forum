using Xunit.Abstractions;
using Xunit.Sdk;

namespace Forum.IntegrationTests;

public class TestOrderer : ITestCollectionOrderer
{
    public IEnumerable<ITestCollection> OrderTestCollections(
        IEnumerable<ITestCollection> testCollections) =>
        testCollections.OrderBy(collection => collection.DisplayName);
}
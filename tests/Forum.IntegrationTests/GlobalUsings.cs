global using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCaseOrderer(
    ordererTypeName: "Forum.IntegrationTests.TestOrderer",
    ordererAssemblyName: "Forum.IntegrationTests")]
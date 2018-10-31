using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.Integration.Camel.Tests
{
    [CollectionDefinition(CollectionTag)]
    public class TestCollectionFixtures : ICollectionFixture<DatabaseFixture>
    {
        public const string CollectionTag = "Database collection";
    }
}
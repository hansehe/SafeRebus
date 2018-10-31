using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.Outbox.Tests
{
    [CollectionDefinition(CollectionTag)]
    public class TestCollectionFixtures : ICollectionFixture<DatabaseFixture>
    {
        public const string CollectionTag = "Database collection";
    }
}
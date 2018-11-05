using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.MessageHandler.Tests
{
    [CollectionDefinition(CollectionTag)]
    public class TestCollectionFixtures : ICollectionFixture<DatabaseFixture>
    {
        public const string CollectionTag = "Database collection";
    }
}
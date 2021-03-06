﻿using SafeRebus.TestUtilities;
using Xunit;

namespace SafeRebus.Adapter.NServiceBus.Tests
{
    [CollectionDefinition(CollectionTag)]
    public class TestCollectionFixtures : ICollectionFixture<DatabaseFixture>
    {
        public const string CollectionTag = "Database collection";
    }
}
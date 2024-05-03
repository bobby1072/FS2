using Common.Models;

namespace DataImporter.MockModelBuilders
{
    internal static class MockGroupCatchBuilder
    {
        private static readonly Random _random = new();
        public static GroupCatch Build(Guid groupId, Guid userId, string? worldFishTaxocode, string? speciesName)
        {
            return new GroupCatch(
                userId,
                groupId,
                speciesName ?? Faker.Name.First(),
                _random.Next(1, 100),
                DateTime.UtcNow,
                _random.Next(0, 100),
                _random.Next(-180, 180),
                _random.Next(-90, 90),
                null,
                Guid.NewGuid(),
                DateTime.UtcNow,
                null,
                null,
                null,
                worldFishTaxocode,
                null
            );
        }
    }
}
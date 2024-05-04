using Common.Models;

namespace DataImporter.MockModelBuilders
{
    internal static class MockGroupCatchBuilder
    {
        public static GroupCatch Build(Guid groupId, Guid userId, string? worldFishTaxocode, string? speciesName)
        {
            var random = new Random();
            return new GroupCatch(
                userId,
                groupId,
                speciesName ?? Faker.Name.First(),
                random.Next(1, 100),
                DateTime.UtcNow,
                random.Next(0, 100),
                random.Next(-180, 180),
                random.Next(-90, 90),
                Faker.Lorem.Sentence(),
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
using System.Text.RegularExpressions;
using Common.Models;

namespace DataImporter.MockModelBuilders
{
    internal static class MockGroupCatchBuilder
    {
        public static GroupCatch Build(Guid groupId, Guid userId, string? worldFishTaxocode = null, string? speciesName = null)
        {
            var random = new Random();
            return new GroupCatch(
                userId,
                groupId,
                speciesName ?? Regex.Replace(Faker.Name.First(), "[^a-zA-Z0-9]", ""),
                random.Next(1, 100),
                DateTime.UtcNow,
                random.Next(1, 100),
                random.Next(-90, 90),
                random.Next(-180, 180),
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
using System.Text.RegularExpressions;
using Common.Models;
using Common.Utils;

namespace DataImporter.MockModelBuilders
{
    public static class MockLiveMatchCatchBuilder
    {
        public static LiveMatchCatch Build(Guid userId, Guid liveMatchId, string? worldFishTaxocode = null, string? speciesName = null)
        {
            var random = new Random();
            return new LiveMatchCatch(
                userId,
                liveMatchId,
                speciesName ?? Regex.Replace(Faker.Name.First(), "[^a-zA-Z0-9]", ""),
                random.Next(1, 100),
                DateTimeUtils.RandomPastDate()(),
                random.Next(1, 100),
                random.Next(-90, 90),
                random.Next(-180, 180),
                Faker.Lorem.Sentence(),
                false,
                Guid.NewGuid(),
                DateTime.UtcNow,
                worldFishTaxocode
            );
        }
    }
}
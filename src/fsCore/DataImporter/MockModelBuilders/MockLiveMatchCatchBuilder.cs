using fsCore.Common.Models;
using fsCore.Common.Utils;
using System.Text.RegularExpressions;

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
                DateTimeUtils.RandomPastDate().Invoke(),
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
        public static LiveMatchCatch Build(Guid userId, Guid liveMatchId, string? speciesName, decimal? length = null, decimal? weight = null, bool valid = false)
        {
            var random = new Random();
            return new LiveMatchCatch(
                userId,
                liveMatchId,
                speciesName ?? Regex.Replace(Faker.Name.First(), "[^a-zA-Z0-9]", ""),
                weight ?? random.Next(1, 100),
                DateTimeUtils.RandomPastDate().Invoke(),
                length ?? random.Next(1, 100),
                random.Next(-90, 90),
                random.Next(-180, 180),
                Faker.Lorem.Sentence(),
                valid,
                Guid.NewGuid(),
                DateTime.UtcNow,
                null
            );
        }
    }
}
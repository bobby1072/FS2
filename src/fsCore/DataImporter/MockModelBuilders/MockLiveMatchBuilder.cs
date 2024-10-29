using Common.Models;

namespace DataImporter.MockModelBuilders
{
    public static class MockLiveMatchBuilder
    {
        public static LiveMatch Build(Guid? leaderId)
        {
            return new LiveMatch(
                Guid.NewGuid(),
                Faker.Company.Name(),
                new LiveMatchRules(),
                LiveMatchStatus.NotStarted,
                LiveMatchWinStrategy.MostCatches,
                new List<LiveMatchCatch>(),
                new List<LiveMatchParticipant>(),
                leaderId ?? Guid.NewGuid(),
                DateTime.UtcNow,
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1),
                Faker.Lorem.Sentence()
            );
        }
    }
}
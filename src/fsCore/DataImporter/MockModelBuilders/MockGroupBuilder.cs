using Common.Models;

namespace DataImporter.MockModelBuilders
{
    internal static class MockGroupBuilder
    {
        private static Random _random = new();
        public static Group Build(Guid leaderId)
        {
            return new Group(
                Faker.Company.Name(),
                null,
                Faker.Lorem.Sentence(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                _random.Next(0, 2) == 1,
                true,
                leaderId
            );
        }
    }
}
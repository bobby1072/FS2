using Common.Models;

namespace DataImporter.MockModelBuilders
{
    internal static class MockGroupBuilder
    {
        public static Group Build(Guid leaderId)
        {
            var random = new Random();
            return new Group(
                Faker.Company.Name(),
                null,
                Faker.Lorem.Sentence(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                random.Next(0, 2) == 1,
                true,
                leaderId
            );
        }
    }
}
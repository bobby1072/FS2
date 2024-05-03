using Common.Models;

namespace DataImporter.ModelBuilders
{
    internal static class GroupBuilder
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
                _random.Next(0, 1) == 1,
                _random.Next(0, 1) == 1,
                leaderId
            );
        }
    }
}
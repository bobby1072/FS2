using System.Text.RegularExpressions;

namespace DataImporter.MockModelBuilders
{
    internal static class MockGroupBuilder
    {
        public static Common.Models.Group Build(Guid leaderId)
        {
            var random = new Random();
            return new Common.Models.Group(
                Regex.Replace(Faker.Name.First(), "[^a-zA-Z0-9]", ""),
                null,
                Faker.Lorem.Sentence(),
                Guid.NewGuid(),
                DateTime.UtcNow,
                random.Next(0, 2) == 1,
                true,
                true,
                leaderId
            );
        }
    }
}
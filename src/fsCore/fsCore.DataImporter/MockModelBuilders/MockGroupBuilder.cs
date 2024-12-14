using System.Text.RegularExpressions;

namespace fsCore.DataImporter.MockModelBuilders
{
    public static class MockGroupBuilder
    {
        public static fsCore.Common.Models.Group Build(Guid leaderId)
        {
            var random = new Random();
            return new fsCore.Common.Models.Group(
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
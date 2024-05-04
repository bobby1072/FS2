using System.Text.RegularExpressions;
using Common.Models;

namespace DataImporter.MockModelBuilders
{
    internal static class MockGroupPositionBuilder
    {
        public static GroupPosition Build(Guid groupId)
        {
            var random = new Random();
            return new GroupPosition(
                groupId,
                Regex.Replace(Faker.Name.First(), "[^a-zA-Z0-9]", ""),
                null,
                random.Next(0, 2) == 1,
                random.Next(0, 2) == 1,
                random.Next(0, 2) == 1,
                random.Next(0, 2) == 1,
                random.Next(0, 2) == 1
            );
        }
    }
}
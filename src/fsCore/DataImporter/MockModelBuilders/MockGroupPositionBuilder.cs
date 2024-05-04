using Common.Models;

namespace DataImporter.MockModelBuilders
{
    internal static class MockGroupPositionBuilder
    {
        private static readonly Random _random = new();
        public static GroupPosition Build(Guid groupId)
        {
            return new GroupPosition(
                groupId,
                Faker.Company.Name(),
                null,
                _random.Next(0, 2) == 1,
                _random.Next(0, 2) == 1,
                _random.Next(0, 2) == 1,
                _random.Next(0, 2) == 1,
                _random.Next(0, 2) == 1
            );
        }
    }
}
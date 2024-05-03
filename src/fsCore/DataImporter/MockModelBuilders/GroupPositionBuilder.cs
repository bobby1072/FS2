using Common.Models;

namespace DataImporter.MockModelBuilders
{
    internal static class GroupPositionBuilder
    {
        private static readonly Random _random = new();
        public static GroupPosition Build(Guid groupId)
        {
            return new GroupPosition(
                groupId,
                Faker.Company.Name(),
                null,
                _random.Next(0, 1) == 1,
                _random.Next(0, 1) == 1,
                _random.Next(0, 1) == 1,
                _random.Next(0, 1) == 1,
                _random.Next(0, 1) == 1
            );
        }
    }
}
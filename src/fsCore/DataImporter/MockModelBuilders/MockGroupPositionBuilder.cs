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
                Faker.Name.First(),
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
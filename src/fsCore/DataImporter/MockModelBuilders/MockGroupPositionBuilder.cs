using System.Text.RegularExpressions;
using Common.Models;

namespace DataImporter.MockModelBuilders
{
    public static class MockGroupPositionBuilder
    {
        public static GroupPosition Build(Guid groupId, bool? canManageGroup, bool? canReadCatches, bool? canManageCatches, bool? canReadMembers, bool? canManageMembers, int? id)
        {
            var random = new Random();
            return new GroupPosition(
                groupId,
                Regex.Replace(Faker.Name.First(), "^a-zA-Z0-9", ""),
                id ?? random.Next(1, 100),
                canManageGroup ?? random.Next(0, 2) == 1,
                canReadCatches ?? random.Next(0, 2) == 1,
                canManageCatches ?? random.Next(0, 2) == 1,
                canReadMembers ?? random.Next(0, 2) == 1,
                canManageMembers ?? random.Next(0, 2) == 1
            );
        }
    }
}
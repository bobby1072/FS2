using Common.Models;

namespace DataImporter.MockModelBuilders
{
    internal static class MockUserBuilder
    {
        public static User Build()
        {
            return new User(
                Faker.Internet.Email(),
                true,
                Faker.Name.FullName(),
                Faker.Internet.UserName()
                );
        }
    }
}
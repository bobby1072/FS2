using Common.Models;

namespace DataImporter.MockModelBuilders
{
    public static class MockUserBuilder
    {
        public static User Build()
        {
            return new User(
                Faker.Internet.Email(),
                true,
                Faker.Name.FullName(),
                Faker.Internet.UserName(),
                Guid.NewGuid()
                );
        }
    }
    public static class MockUserWithPerrmissionsBuilder
    {
        public static UserWithGroupPermissionSet Build(User? user) => new(user ?? MockUserBuilder.Build());
    }
}
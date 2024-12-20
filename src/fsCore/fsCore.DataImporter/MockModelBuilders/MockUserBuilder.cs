using fsCore.Common.Models;

namespace fsCore.DataImporter.MockModelBuilders
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
    public static class MockUserWithPermissionsBuilder
    {
        public static UserWithGroupPermissionSet Build(User? user = null) => new(user ?? MockUserBuilder.Build());
    }
}
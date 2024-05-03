using Common.Models;

namespace DataImporter.MockModelBuilders
{
    internal class UserBuilder
    {
        public User Build()
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
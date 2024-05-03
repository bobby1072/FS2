using Common.Models;

namespace DataImporter.ModelBuilders
{
    internal class UserBuilder
    {
        public User BuildModel()
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
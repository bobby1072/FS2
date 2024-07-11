using Common.Models;
using Common.Models.Validators;
using DataImporter.MockModelBuilders;
using Microsoft.VisualBasic;
namespace fsCore.test.ServiceTests.ValidatorTests
{
    public class UserValidatorTest
    {
        private UserValidator _validator;
        public UserValidatorTest()
        {
            _validator = new UserValidator();
        }
        internal class User_Class_Data : TheoryData<User, bool>
        {
            public User_Class_Data()
            {
                var userWithEmptyEmail = MockUserBuilder.Build();
                userWithEmptyEmail.Email = "";
                Add(userWithEmptyEmail, false);

                var validUser = MockUserBuilder.Build();
                validUser.Email = Faker.Internet.Email();
                validUser.Username = Faker.Internet.UserName();
                Add(validUser, true);

                var userWithInvalidEmail = MockUserBuilder.Build();
                userWithInvalidEmail.Email = "invalid-email";
                Add(userWithInvalidEmail, false);

                var userWithNullEmail = MockUserBuilder.Build();
                userWithNullEmail.Email = null;
                Add(userWithNullEmail, false);

                var userWithNumericUsername = MockUserBuilder.Build();
                userWithNumericUsername.Username = "123456";
                Add(userWithNumericUsername, false);

                var userWithNullUsername = MockUserBuilder.Build();
                userWithNullUsername.Username = null;
                Add(userWithNullUsername, false);

                var userWithEmptyUsername = MockUserBuilder.Build();
                userWithEmptyUsername.Username = "";
                Add(userWithEmptyUsername, false);

                var userWithValidEmailInvalidUsername = MockUserBuilder.Build();
                userWithValidEmailInvalidUsername.Email = Faker.Internet.Email();
                userWithValidEmailInvalidUsername.Username = "12345";
                Add(userWithValidEmailInvalidUsername, false);

                var userWithInvalidEmailValidUsername = MockUserBuilder.Build();
                userWithInvalidEmailValidUsername.Email = "invalid-email";
                userWithInvalidEmailValidUsername.Username = Faker.Internet.UserName();
                Add(userWithInvalidEmailValidUsername, false);

                var userWithSpecialCharUsername = MockUserBuilder.Build();
                userWithSpecialCharUsername.Email = Faker.Internet.Email();
                userWithSpecialCharUsername.Username = "user@name";
                Add(userWithSpecialCharUsername, true);

                var userWithLongUsername = MockUserBuilder.Build();
                userWithLongUsername.Email = Faker.Internet.Email();
                userWithLongUsername.Username = new string('a', 256);
                Add(userWithLongUsername, false);

            }
        }
        [Theory]
        [ClassData(typeof(User_Class_Data))]
        public void User_Validator_Should_Validate_Correctly(User user, bool expected)
        {
            var result = _validator.Validate(user);

            Assert.Equal(expected, result.IsValid);
        }
    }
}
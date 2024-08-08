using DataImporter.MockModelBuilders;
using Common.Models;
using Common.Models.Validators;

namespace fsCore.Tests.ModelTests.ValidatorTests
{
    public class GroupValidatorTest : TestBase
    {
        private readonly GroupValidator _validator;
        public GroupValidatorTest()
        {
            _validator = new GroupValidator();
        }
        internal class Group_Class_Data : TheoryData<Group, bool>
        {
            public Group_Class_Data()
            {
                var validGroup = MockGroupBuilder.Build(Guid.NewGuid());
                Add(validGroup, true);

                // Group with empty name
                var groupWithEmptyName = MockGroupBuilder.Build(Guid.NewGuid());
                groupWithEmptyName.Name = "";
                Add(groupWithEmptyName, false);

                // Group with null name
                var groupWithNullName = MockGroupBuilder.Build(Guid.NewGuid());
                groupWithNullName.Name = null;
                Add(groupWithNullName, false);

                // Group with null description
                var groupWithNullDescription = MockGroupBuilder.Build(Guid.NewGuid());
                groupWithNullDescription.Description = null;
                Add(groupWithNullDescription, true);

                // Group with empty description
                var groupWithEmptyDescription = MockGroupBuilder.Build(Guid.NewGuid());
                groupWithEmptyDescription.Description = "";
                Add(groupWithEmptyDescription, true);

                // Group with null leaderId


                // Group with future createdAt date
                var groupWithFutureCreatedAt = MockGroupBuilder.Build(Guid.NewGuid());
                groupWithFutureCreatedAt.CreatedAt = DateTime.UtcNow.AddDays(1);
                Add(groupWithFutureCreatedAt, false);

                // Group with special characters in name
                var groupWithSpecialCharName = MockGroupBuilder.Build(Guid.NewGuid());
                groupWithSpecialCharName.Name = "Group with special characters !@#$%^&*()_+{}|:<>?~`-=[]\\;',./\"";
                Add(groupWithSpecialCharName, false);

                // Group with overly long name
                var groupWithLongName = MockGroupBuilder.Build(Guid.NewGuid());
                groupWithLongName.Name = new string('a', 256);
                Add(groupWithLongName, false);

                // Group with valid emblem
                var groupWithValidEmblem = MockGroupBuilder.Build(Guid.NewGuid());
                groupWithValidEmblem.Emblem = new byte[1];
                Add(groupWithValidEmblem, true);
            }
        }
        [Theory]
        [ClassData(typeof(Group_Class_Data))]
        public void Group_Validator_Should_Validate_Correctly(Group group, bool expected)
        {
            var result = _validator.Validate(group);

            Assert.Equal(expected, result.IsValid);
        }
    }
}
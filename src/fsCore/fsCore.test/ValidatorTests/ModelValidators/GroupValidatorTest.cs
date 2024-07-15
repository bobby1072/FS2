using System.Text.RegularExpressions;
using Common.Models.Validators;

namespace fsCore.test.ValidatorTests.ModelValidators
{
    public class GroupValidatorTest
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

            }
        }
        [Theory]
        [ClassData(typeof(Group_Class_Data))]
        public async Task Group_Validator_Should_Validate_Correctly(Group group, bool expected)
        {

        }
    }
}
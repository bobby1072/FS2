using fsCore.Common.Models;
using fsCore.Common.Models.Validators;

namespace fsCore.Tests.ModelTests.ValidatorTests
{
    public class SpecificSpeciesMatchCatchRuleValidatorTests : TestBase
    {
        private SpecificSpeciesMatchCatchRuleValidator _validator;

        public SpecificSpeciesMatchCatchRuleValidatorTests()
        {
            _validator = new SpecificSpeciesMatchCatchRuleValidator();
        }

        private class SpecificSpeciesMatchCatch_Should_Validate_Correctly_Class_Data
            : TheoryData<SpecificSpeciesLiveMatchCatchRule, bool>
        {
            public SpecificSpeciesMatchCatch_Should_Validate_Correctly_Class_Data()
            {
                var validRule = new SpecificSpeciesLiveMatchCatchRule(
                    new List<string> { "Salmon", "Trout" },
                    new List<WorldFish>
                    {
                        new WorldFish("001", null, null, null, "Salmon", null),
                        new WorldFish("002", null, null, null, "Trout", null),
                    }
                );
                Add(validRule, true);

                // Rule with empty SpeciesNames
                var emptySpeciesNames = new SpecificSpeciesLiveMatchCatchRule(
                    new List<string>(),
                    new List<WorldFish>()
                );
                Add(emptySpeciesNames, false);

                // Rule with non-unique SpeciesNames
                var nonUniqueSpeciesNames = new SpecificSpeciesLiveMatchCatchRule(
                    new List<string> { "Salmon", "Salmon" },
                    new List<WorldFish> { new WorldFish("001", null, null, null, "Salmon", null) }
                );
                Add(nonUniqueSpeciesNames, false);

                // Rule with empty species name string
                var emptySpeciesNameString = new SpecificSpeciesLiveMatchCatchRule(
                    new List<string> { "Salmon", "" },
                    new List<WorldFish> { new WorldFish("001", null, null, null, "Salmon", null) }
                );
                Add(emptySpeciesNameString, false);

                // Rule where WorldFish does not match SpeciesNames
                var mismatchedWorldFish = new SpecificSpeciesLiveMatchCatchRule(
                    new List<string> { "Salmon", "Trout" },
                    new List<WorldFish>
                    {
                        new WorldFish("001", null, null, null, "Salmon", null),
                        new WorldFish("003", null, null, null, "Bass", null),
                    }
                );
                Add(mismatchedWorldFish, false);

                // Rule with special characters in species names
                var specialCharSpeciesNames = new SpecificSpeciesLiveMatchCatchRule(
                    new List<string> { "Salmon", "Tr@ut" },
                    new List<WorldFish> { new WorldFish("001", null, null, null, "Salmon", null) }
                );
                Add(specialCharSpeciesNames, false);

                // Rule with valid species names and WorldFish not empty
                var validSpeciesNamesAndWorldFish = new SpecificSpeciesLiveMatchCatchRule(
                    new List<string> { "Salmon", "Trout" },
                    new List<WorldFish>
                    {
                        new WorldFish("001", null, null, null, "Salmon", null),
                        new WorldFish("002", null, null, null, "Trout", null),
                    }
                );
                Add(validSpeciesNamesAndWorldFish, true);
            }
        }

        [Theory]
        [ClassData(typeof(SpecificSpeciesMatchCatch_Should_Validate_Correctly_Class_Data))]
        public void SpecificSpeciesMatchCatch_Should_Validate_Correctly(
            SpecificSpeciesLiveMatchCatchRule rule,
            bool expected
        )
        {
            //Act
            var result = _validator.Validate(rule);

            //Assert
            Assert.Equal(expected, result.IsValid);
        }
    }
}

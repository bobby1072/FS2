using Common.Models;
using Common.Models.Validators;

namespace fsCore.Tests.ModelTests.ValidatorTests
{
    public class SpecificSpeciesMatchCatchRuleValidatorTests
    {
        private SpecificSpeciesMatchCatchRuleValidator _validator;
        public SpecificSpeciesMatchCatchRuleValidatorTests()
        {
            _validator = new SpecificSpeciesMatchCatchRuleValidator();
        }
        public void SpecificSpeciesMatchCatch_Should_Validate_Correctly(SpecificSpeciesLiveMatchCatchRule rule, bool expected)
        {

        }
    }
}
using FluentValidation;

namespace Common.Models.Validators
{
    public class DynamicLiveMatchRuleValidator : LiveMatchCatchValidator
    {
        public DynamicLiveMatchRuleValidator(ICollection<LiveMatchCatchSingleRuleValidatorFunction> rules) : base()
        {
            for (int i = 0; i < rules.Count; i++)
            {
                var validatorFunction = rules.ElementAt(i);
                RuleFor(x => x).Must(validatorFunction.ValidatorFunction).WithMessage(validatorFunction.ErrorMessage);
            }
        }
    }
}
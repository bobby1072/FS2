using FluentValidation;

namespace Common.Models.Validators
{
    public class RuleValidator : AbstractValidator<LiveMatchCatch>, IValidator<LiveMatchCatch>
    {
        public RuleValidator(ICollection<LiveMatchCatchSingleRuleValidatorFunction> rules)
        {
            for (int i = 0; i < rules.Count; i++)
            {
                var validatorFunction = rules.ElementAt(i);
                RuleFor(x => x).Must(validatorFunction.ValidatorFunction).WithMessage(validatorFunction.ErrorMessage);
            }
        }
    }
}
using FluentValidation;

namespace Common.Models.Validators
{
    public class DynamicLiveMatchCatchRuleValidatorForSingle : LiveMatchCatchValidator
    {
        public DynamicLiveMatchCatchRuleValidatorForSingle(ICollection<(Func<LiveMatchCatch, bool> ValidatorFunctionForSingle, string ErrorMessage)> rules) : base()
        {
            for (int i = 0; i < rules.Count; i++)
            {
                var (ValidatorFunctionForSingle, ErrorMessage) = rules.ElementAt(i);
                RuleFor(x => x).Must(ValidatorFunctionForSingle).WithMessage(ErrorMessage);
            }
        }
    }
    public class DynamicLiveMatchCatchRuleValidatorForEnumerable : AbstractValidator<IEnumerable<LiveMatchCatch>>
    {
        public DynamicLiveMatchCatchRuleValidatorForEnumerable(ICollection<(Func<IEnumerable<LiveMatchCatch>, bool> ValidatorFunctionForSingle, string ErrorMessage)> rules) : base()
        {
            for (int i = 0; i < rules.Count; i++)
            {
                var (ValidatorFunctionForSingle, ErrorMessage) = rules.ElementAt(i);
                RuleFor(x => x).Must(ValidatorFunctionForSingle).WithMessage(ErrorMessage);
            }
        }
    }
}
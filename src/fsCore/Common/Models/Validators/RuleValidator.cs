using FluentValidation;

namespace Common.Models.Validators
{
    public class RuleValidator : AbstractValidator<MatchCatch>, IValidator<MatchCatch>
    {
        public RuleValidator(ICollection<(Func<MatchCatch, bool>, string)> Rules)
        {
            for (int i = 0; i < Rules.Count; i++)
            {
                var (ruleFunc, errorMessage) = Rules.ElementAt(i);
                RuleFor(x => x).Must(ruleFunc).WithMessage(errorMessage);
            }
        }
    }
}
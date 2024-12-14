using FluentValidation;
using fsCore.Common.Misc;

namespace fsCore.Common.Models.Validators
{
    public class LiveMatchValidator : BaseValidator<LiveMatch>
    {
        public LiveMatchValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.MatchName).Must(NumbersLettersAndWhitespaceOnlyNotJustWhiteSpaceOrNumbers).WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.MatchName).Must(ShouldBeLessThanOrEqualTo(50)).WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.MatchRules).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.MatchStatus).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.MatchWinStrategy).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.Catches).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.Participants).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.MatchLeaderId).NotEmpty().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x).Must(LiveMatchLeaderIsInParticipants).WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.CreatedAt).Must(DateInThePastOrNow).WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.CommencesAt).Must(x => x is null || DateInTheFuture((DateTime)x)).WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.EndsAt).Must(x => x is null || DateInTheFuture((DateTime)x)).WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x).Must(x => MustBeBefore(x.CommencesAt, x.EndsAt)).WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
        }
        private static bool LiveMatchLeaderIsInParticipants(LiveMatch liveMatch)
        {
            return liveMatch.Participants.Any(p => p.Id == liveMatch.MatchLeaderId);
        }
    }
    public class LiveMatchDIValidator : LiveMatchValidator
    {
        private readonly IValidator<SpecificSpeciesLiveMatchCatchRule> _specificCatchRuleValidator;
        private readonly IValidator<InAreaLiveMatchCatchRule> _inAreaRuleValidator;
        public LiveMatchDIValidator(IValidator<SpecificSpeciesLiveMatchCatchRule> specificCatchRuleValidator, IValidator<InAreaLiveMatchCatchRule> inAreaRuleValidator) : base()
        {
            _specificCatchRuleValidator = specificCatchRuleValidator;
            _inAreaRuleValidator = inAreaRuleValidator;
            RuleFor(x => x).Must(ValidateRules).WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
        }
        private bool ValidateRules(LiveMatch match)
        {
            foreach (var rule in match.MatchRules.Rules)
            {
                if (rule is SpecificSpeciesLiveMatchCatchRule specificSpeciesRule)
                {
                    if (!_specificCatchRuleValidator.Validate(specificSpeciesRule).IsValid) return false;
                }
                else if (rule is InAreaLiveMatchCatchRule inAreaRule)
                {
                    if (!_inAreaRuleValidator.Validate(inAreaRule).IsValid) return false;
                }
            }
            return true;
        }
    }
}
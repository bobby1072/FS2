using FluentValidation;

namespace Common.Models.Validators
{
    public class LiveMatchValidator : BaseValidator<LiveMatch>
    {
        public LiveMatchValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty().WithMessage(LiveMatchConstants.LiveMatchHasMissingDetails);
            RuleFor(x => x.MatchName).NotEmpty().WithMessage(LiveMatchConstants.LiveMatchHasMissingDetails);
            RuleFor(x => x.MatchRules).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingDetails);
            RuleFor(x => x.MatchStatus).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingDetails);
            RuleFor(x => x.MatchWinStrategy).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingDetails);
            RuleFor(x => x.Catches).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingDetails);
        }
    }
}
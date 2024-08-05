using FluentValidation;

namespace Common.Models.Validators
{
    public class LiveMatchValidator : BaseValidator<LiveMatch>
    {
        public LiveMatchValidator()
        {
            RuleFor(x => x.GroupId).NotEmpty().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.MatchName).Must(NumbersLettersAndWhitespaceOnlyNotJustWhiteSpaceOrNumbers).WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.MatchName).Must(ShouldBeLength(50)).WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.MatchRules).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.MatchStatus).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.MatchWinStrategy).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.Catches).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.Participants).NotNull().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x.MatchLeaderId).NotEmpty().WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
            RuleFor(x => x).Must(LiveMatchLeaderIsInParticipants).WithMessage(LiveMatchConstants.LiveMatchHasMissingOrIncorrectDetails);
        }
        private static bool LiveMatchLeaderIsInParticipants(LiveMatch liveMatch)
        {
            return liveMatch.Participants.Any(p => p.Id == liveMatch.MatchLeaderId);
        }
    }
}
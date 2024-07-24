using FluentValidation;

namespace Common.Models.Validators
{
    public class LiveMatchCatchValidator : CatchValidator<LiveMatchCatch>, IValidator<LiveMatchCatch>
    {
        public LiveMatchCatchValidator() : base()
        {
        }
    }
}
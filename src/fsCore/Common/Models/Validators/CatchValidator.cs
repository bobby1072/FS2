using FluentValidation;

namespace Common.Models.Validators
{
    public abstract class CatchValidator<T> : BaseValidator<T>, IValidator<T> where T : Catch
    {
        protected CatchValidator()
        {
            RuleFor(x => x.Species).NotEmpty().WithMessage(ErrorConstants.InvalidSpeciesInCatch);
            RuleFor(x => x.Species).Must(LettersAndWhiteSpaceOnly).WithMessage(ErrorConstants.InvalidSpeciesInCatch);
            RuleFor(x => x.Description).Must(NotJustBeWhiteSpace).WithMessage(ErrorConstants.InvalidDescription);
            RuleFor(x => x.Description).Must(NotJustHaveNumbers).WithMessage(ErrorConstants.InvalidDescription);
            RuleFor(x => x.Latitude).Must(LatWithInRange).WithMessage(ErrorConstants.InvalidLatitudeAndLongitude);
            RuleFor(x => x.Longitude).Must(LngWithInRange).WithMessage(ErrorConstants.InvalidLatitudeAndLongitude);
            RuleFor(x => x.Length).Must(NumbersArePositive).WithMessage(ErrorConstants.InvalidLength);
            RuleFor(x => x.Weight).Must(NumbersArePositive).WithMessage(ErrorConstants.InvalidWeight);
            RuleFor(x => x.CaughtAt).Must(DateInThePastOrNow).WithMessage(ErrorConstants.DateCaughtMustBeInThePast);
            RuleFor(x => x.CreatedAt).Must(DateInThePastOrNow).WithMessage(ErrorConstants.DateCaughtMustBeInThePast);
        }
    }
}
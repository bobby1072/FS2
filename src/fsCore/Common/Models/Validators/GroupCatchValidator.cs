using FluentValidation;

namespace Common.Models.Validators
{
    public class GroupCatchValidator : BaseValidator<GroupCatch>, IValidator<GroupCatch>
    {
        public GroupCatchValidator()
        {
            RuleFor(x => x.Species).NotEmpty().WithMessage(ErrorConstants.InvalidGroupCatchSpecies);
            RuleFor(x => x.Species).Must(LettersAndWhiteSpaceOnly).WithMessage(ErrorConstants.InvalidGroupCatchSpecies);
            RuleFor(x => x.Description).Must(NotJustBeWhiteSpace).WithMessage(ErrorConstants.InvalidDescription);
            RuleFor(x => x.Description).Must(NotJustHaveNumbers).WithMessage(ErrorConstants.InvalidDescription);
            RuleFor(x => x.Latitude).Must(LatWithInRange).WithMessage(ErrorConstants.InvalidLatitudeAndLongitude);
            RuleFor(x => x.Longitude).Must(LngWithInRange).WithMessage(ErrorConstants.InvalidLatitudeAndLongitude);
            RuleFor(x => x.Length).Must(NumbersArePositive).WithMessage(ErrorConstants.InvalidLength);
            RuleFor(x => x.Weight).Must(NumbersArePositive).WithMessage(ErrorConstants.InvalidWeight);
            RuleFor(x => x.CaughtAt).Must(DateInThePast).WithMessage(ErrorConstants.DateCaughtMustBeInThePast);
        }
    }
}
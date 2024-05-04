using FluentValidation;

namespace Common.Models.Validators
{
    public class GroupCatchValidator : BaseValidator<GroupCatch>
    {
        public GroupCatchValidator()
        {
            RuleFor(x => x.Species).Must(LettersAndWhiteSpaceOnly).WithMessage(ErrorConstants.InvalidGroupCatchSpecies);
            RuleFor(x => x.Description).Must(NotJustBeWhiteSpace).WithMessage(ErrorConstants.InvalidGroupCatch);
            RuleFor(x => x.Description).Must(NotJustHaveNumbers).WithMessage(ErrorConstants.InvalidGroupCatch);
            RuleFor(x => x.Latitude).Must(LatWithInRange).WithMessage(ErrorConstants.InvalidLatitudeAndLongitude);
            RuleFor(x => x.Longitude).Must(LngWithInRange).WithMessage(ErrorConstants.InvalidLatitudeAndLongitude);
            RuleFor(x => x.Length).Must(NumbersArePositive).WithMessage(ErrorConstants.NumbersMustBePositive);
            RuleFor(x => x.Weight).Must(NumbersArePositive).WithMessage(ErrorConstants.NumbersMustBePositive);
        }

    }
}
using FluentValidation;

namespace Common.Models.Validators
{
    public class GroupCatchValidator : BaseValidator<GroupCatch>
    {
        public GroupCatchValidator()
        {
            RuleFor(x => x.Species).Must(_lettersAndWhiteSpaceOnly).WithMessage(ErrorConstants.InvalidGroupCatchSpecies);
            RuleFor(x => x.Description).Must(_notJustBeWhiteSpace).WithMessage(ErrorConstants.InvalidGroupCatch);
            RuleFor(x => x.Description).Must(_notJustHaveNumbers).WithMessage(ErrorConstants.InvalidGroupCatch);
            RuleFor(x => x.Latitude).Must(_LatWithInRange).WithMessage(ErrorConstants.InvalidLatitudeAndLongitude);
            RuleFor(x => x.Longitude).Must(_LngWithInRange).WithMessage(ErrorConstants.InvalidLatitudeAndLongitude);
        }

    }
}
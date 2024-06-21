using FluentValidation;

namespace Common.Models.Validators
{
    public class GroupPositionValidator : BaseValidator<GroupPosition>, IValidator<GroupPosition>
    {
        public GroupPositionValidator()
        {
            RuleFor(x => x.Name).Must(LettersAndWhiteSpaceOnly).WithMessage(ErrorConstants.InvalidGroupPositionName);
        }
    }
}
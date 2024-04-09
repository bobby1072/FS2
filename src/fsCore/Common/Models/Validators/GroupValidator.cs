using FluentValidation;

namespace Common.Models.Validators
{
    public class GroupValidator : BaseValidator<Group>
    {
        public GroupValidator()
        {
            RuleFor(x => x.Name).Must(_lettersAndWhiteSpaceOnly).WithMessage(ErrorConstants.GroupNameCorrectFormat);

            RuleFor(x => x.Description).Must(_notJustHaveNumbers).WithMessage(ErrorConstants.InvalidGroupDescription);
            RuleFor(x => x.Description).Must(_notJustBeWhiteSpace).WithMessage(ErrorConstants.InvalidGroupDescription);
        }
    }
}
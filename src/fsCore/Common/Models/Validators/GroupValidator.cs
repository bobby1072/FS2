using FluentValidation;

namespace Common.Models.Validators
{
    public class GroupValidator : BaseValidator<Group>
    {
        public GroupValidator()
        {
            RuleFor(x => x.Name).Must(_notJustBeWhiteSpace).WithMessage(ErrorConstants.GroupNameCorrectFormat);
            RuleFor(x => x.Name).Must(_notJustHaveNumbers).WithMessage(ErrorConstants.GroupNameCorrectFormat);

            RuleFor(x => x.Description).Must(_notJustHaveNumbers).WithMessage(ErrorConstants.InvalidGroupDescription);
            RuleFor(x => x.Description).Must(_notJustBeWhiteSpace).WithMessage(ErrorConstants.InvalidGroupDescription);
        }
    }
}
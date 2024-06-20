using FluentValidation;

namespace Common.Models.Validators
{
    public class GroupValidator : BaseValidator<Group>
    {
        public GroupValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(ErrorConstants.GroupNameCorrectFormat);
            RuleFor(x => x.Name).Must(NotHaveNonAlphanumerics).WithMessage(ErrorConstants.GroupNameCorrectFormat);
            RuleFor(x => x.Name).Must(NotJustHaveNumbers).WithMessage(ErrorConstants.GroupNameCorrectFormat);

            RuleFor(x => x.Description).Must(NotJustHaveNumbers).WithMessage(ErrorConstants.InvalidGroupDescription);
            RuleFor(x => x.Description).Must(NotJustBeWhiteSpace).WithMessage(ErrorConstants.InvalidGroupDescription);
        }
    }
}
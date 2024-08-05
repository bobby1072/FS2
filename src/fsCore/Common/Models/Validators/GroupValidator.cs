using FluentValidation;

namespace Common.Models.Validators
{
    public class GroupValidator : BaseValidator<Group>
    {
        public GroupValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(ErrorConstants.GroupNameCorrectFormat);
            RuleFor(x => x.Name).Must(ShouldBeLessThanOrEqualTo(50)).WithMessage(ErrorConstants.GroupNameCorrectFormat);
            RuleFor(x => x.Name).Must(NumbersLettersAndWhitespaceOnlyNotJustWhiteSpaceOrNumbers).WithMessage(ErrorConstants.GroupNameCorrectFormat);

            RuleFor(x => x.CreatedAt).Must(DateInThePastOrNow).WithMessage(ErrorConstants.DateMustBeInThePast);

            RuleFor(x => x.Description).Must(NotJustHaveNumbers).WithMessage(ErrorConstants.InvalidGroupDescription);
            RuleFor(x => x.Description).Must(NotJustBeWhiteSpace).WithMessage(ErrorConstants.InvalidGroupDescription);
        }
    }
}
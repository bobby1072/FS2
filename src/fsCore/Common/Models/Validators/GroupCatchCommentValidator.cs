using FluentValidation;

namespace Common.Models.Validators
{
    public class GroupCatchCommentValidator : BaseValidator<GroupCatchComment>
    {
        public GroupCatchCommentValidator()
        {
            RuleFor(x => x.Comment).NotEmpty().WithMessage(ErrorConstants.InvalidCatchComment);
            RuleFor(x => x.Comment).Must(ShouldBeLessThanOrEqualTo(250)).WithMessage(ErrorConstants.InvalidCatchComment);
        }
    }
}

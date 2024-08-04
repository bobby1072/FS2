using FluentValidation;

namespace Common.Models.Validators
{
    public class GroupCatchCommentValidator : BaseValidator<GroupCatchComment>
    {
        public GroupCatchCommentValidator()
        {
            RuleFor(x => x.Comment).NotEmpty().WithMessage(ErrorConstants.InvalidCatchComment);
            RuleFor(x => x.Comment.Length > 250).NotEmpty().WithMessage(ErrorConstants.InvalidCatchComment);
        }
    }
}

using FluentValidation;

namespace Common.Models.Validators
{
    public class GroupCatchCommentValidator : BaseValidator<GroupCatchComment>, IValidator<GroupCatchComment>
    {
        public GroupCatchCommentValidator()
        {
            RuleFor(x => x.Comment).NotEmpty().WithMessage(ErrorConstants.InvalidCatchComment);
        }
    }
}

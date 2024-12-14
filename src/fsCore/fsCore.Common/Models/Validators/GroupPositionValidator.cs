using FluentValidation;
using fsCore.Common.Misc;
using fsCore.Common.Models;

namespace fsCore.Common.Models.Validators
{
    public class GroupPositionValidator : BaseValidator<GroupPosition>
    {
        public GroupPositionValidator()
        {
            RuleFor(x => x.Name).Must(LettersAndWhiteSpaceOnly).WithMessage(ErrorConstants.InvalidGroupPositionName);
        }
    }
}
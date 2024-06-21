using FluentValidation;
namespace Common.Models.Validators
{
    public class UserValidator : BaseValidator<User>, IValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(ErrorConstants.InvalidEmail);
            RuleFor(x => x.Email).Must(NotHaveInvalidEmail).WithMessage(ErrorConstants.InvalidEmail);

            RuleFor(x => x.Username).NotEmpty().WithMessage(ErrorConstants.UsernameInvalid);
            RuleFor(x => x.Username).Must(NotJustHaveNumbers).WithMessage(ErrorConstants.UsernameInvalid);
        }
    }
}
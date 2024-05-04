using FluentValidation;
namespace Common.Models.Validators
{
    public class UserValidator : BaseValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).Must(NotHaveWhitespaceOrBeEmpty).WithMessage(ErrorConstants.InvalidUserEmail);
            RuleFor(x => x.Email).Must(NotHaveInvalidEmail).WithMessage(ErrorConstants.InvalidEmail);

            RuleFor(x => x.Username).Must(NotHaveWhitespaceOrBeEmpty).WithMessage(ErrorConstants.UsernameInvalid);
            RuleFor(x => x.Username).Must(NotJustHaveNumbers).WithMessage(ErrorConstants.UsernameInvalid);
        }
    }
}
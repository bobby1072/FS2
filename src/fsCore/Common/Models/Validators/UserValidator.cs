using FluentValidation;
namespace Common.Models.Validators
{
    public class UserValidator : BaseValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).Must(_notHaveWhitespaceOrBeEmpty).WithMessage(ErrorConstants.InvalidUserEmail);
            RuleFor(x => x.Email).Must(_notHaveInvalidEmail).WithMessage(ErrorConstants.InvalidEmail);

            RuleFor(x => x.Username).Must(_notHaveWhitespaceOrBeEmpty).WithMessage(ErrorConstants.UsernameInvalid);
            RuleFor(x => x.Username).Must(_notJustHaveNumbers).WithMessage(ErrorConstants.UsernameInvalid);
        }
    }
}
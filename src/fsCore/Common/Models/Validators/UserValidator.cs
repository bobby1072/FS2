using FluentValidation;
namespace Common.Models.Validators
{
    public class UserValidator : BaseValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).Must(_notHaveWhitespaceOrBeEmpty).WithMessage(ErrorConstants.InvalidUserEmail);
            RuleFor(x => x.Email).Must(_notHaveInvalidEmail).WithMessage(ErrorConstants.InvalidEmail);

            RuleFor(x => x.Username).Must(_notHaveNonAlphanumerics).WithMessage(ErrorConstants.UsernameCorrectFormat);
            RuleFor(x => x.Username).Must(_notHaveWhitespaceOrBeEmpty).WithMessage(ErrorConstants.UsernameCorrectFormat);
            RuleFor(x => x.Username).Must(_notJustHaveNumbers).WithMessage(ErrorConstants.UsernameCorrectFormat);
        }
    }
}
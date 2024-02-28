using Common.Utils;
using FluentValidation;

namespace Common.Models.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T> where T : BaseModel
    {
        protected bool _notHaveWhitespaceOrBeEmpty(string input) => !string.IsNullOrWhiteSpace(input) && !input.Contains(' ');
        protected bool _notHaveNonAlphanumerics(string input) => !input.All(char.IsLetterOrDigit);
        protected bool _notHaveInvalidEmail(string input) => input.IsValidEmail();
    }
}
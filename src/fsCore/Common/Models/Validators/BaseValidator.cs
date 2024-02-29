using Common.Utils;
using FluentValidation;

namespace Common.Models.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T> where T : BaseModel
    {
        protected virtual bool _notHaveWhitespaceOrBeEmpty(string input) => !string.IsNullOrWhiteSpace(input) && !input.Contains(' ');
        protected virtual bool _notHaveNonAlphanumerics(string input) => input.All(char.IsLetterOrDigit);
        protected virtual bool _notHaveInvalidEmail(string input) => input.IsValidEmail();
        protected virtual bool _notJustBeWhiteSpace(string? input) => string.IsNullOrEmpty(input) ? true : !input.All(char.IsWhiteSpace);
        protected virtual bool _notJustHaveNumbers(string? input) => string.IsNullOrEmpty(input) ? true : !input.All(char.IsDigit);
    }
}
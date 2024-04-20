using Common.Utils;
using FluentValidation;

namespace Common.Models.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T> where T : class
    {
        protected static bool _LatWithInRange(double lat)
        {
            return lat >= -90 && lat <= 90;
        }
        protected static bool _LngWithInRange(double lng)
        {
            return lng >= -180 && lng <= 180;
        }
        protected static bool _notHaveWhitespaceOrBeEmpty(string input) => !string.IsNullOrWhiteSpace(input) && !input.Contains(' ');
        protected static bool _notHaveNonAlphanumerics(string input) => input.All(char.IsLetterOrDigit);
        protected static bool _notHaveInvalidEmail(string input) => input.IsValidEmail();
        protected static bool _notJustBeWhiteSpace(string? input) => string.IsNullOrEmpty(input) ? true : !input.All(char.IsWhiteSpace);
        protected static bool _notJustHaveNumbers(string? input) => string.IsNullOrEmpty(input) ? true : !input.All(char.IsDigit);
        protected static bool _lettersAndWhiteSpaceOnly(string input) => input.Any(char.IsLetter) && !input.All(char.IsWhiteSpace) && !input.Any(char.IsDigit) && !input.Any(char.IsPunctuation);
    }
}
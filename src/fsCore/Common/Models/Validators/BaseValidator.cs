using Common.Utils;
using FluentValidation;

namespace Common.Models.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T> where T : class
    {

        protected static bool GuidNotNullOrEmpty(Guid? guid) => guid.HasValue && guid != Guid.Empty;
        protected static bool LatWithInRange(double lat)
        {
            return lat >= -90 && lat <= 90;
        }
        protected static bool LngWithInRange(double lng)
        {
            return lng >= -180 && lng <= 180;
        }
        protected static bool DateInThePastOrNow(DateTime date) => date <= DateTime.UtcNow;
        protected static bool NotHaveNonAlphanumerics(string? input) => input?.All(char.IsLetterOrDigit) ?? false;
        protected static bool NotHaveInvalidEmail(string input) => input.IsValidEmail();
        protected static bool NotJustBeWhiteSpace(string? input) => string.IsNullOrEmpty(input) ? true : !input.All(char.IsWhiteSpace);
        protected static bool NotJustHaveNumbers(string? input) => string.IsNullOrEmpty(input) ? true : !input.All(char.IsDigit);
        protected static bool LettersAndWhiteSpaceOnly(string input) => input.Any(char.IsLetter) && !input.All(char.IsWhiteSpace) && !input.Any(char.IsDigit) && !input.Any(char.IsPunctuation);
        protected static bool NumbersArePositive(double input) => input > 0;
        protected static bool NumbersLettersAndWhitespaceOnlyNotJustWhiteSpace(string? input)
        {
            return !string.IsNullOrEmpty(input) && input.All(x => x == char.Parse(" ") || char.IsLetterOrDigit(x)) && !input.All(char.IsWhiteSpace);
        }
    }
}
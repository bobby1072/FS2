using Common.Utils;
using FluentValidation;

namespace Common.Models.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T> where T : class
    {

        protected static bool GuidNotNullOrEmpty(Guid? guid) => guid.HasValue && guid != Guid.Empty;
        protected static bool LatWithInRange(decimal lat)
        {
            return lat >= -90 && lat <= 90;
        }
        protected static bool LngWithInRange(decimal lng)
        {
            return lng >= -180 && lng <= 180;
        }
        protected static bool DateInTheFuture(DateTime date) => date > DateTime.UtcNow;
        protected static bool DateInThePastOrNow(DateTime date) => date <= DateTime.UtcNow;
        protected static bool NotHaveNonAlphanumerics(string? input) => input?.All(char.IsLetterOrDigit) ?? false;
        protected static bool NotHaveInvalidEmail(string input) => input.IsValidEmail();
        protected static bool NotJustBeWhiteSpace(string? input) => string.IsNullOrEmpty(input) ? true : !input.All(char.IsWhiteSpace);
        protected static bool NotJustHaveNumbers(string? input) => string.IsNullOrEmpty(input) ? true : !input.All(char.IsDigit);
        protected static bool LettersAndWhiteSpaceOnly(string input) => input.Any(char.IsLetter) && !input.All(char.IsWhiteSpace) && !input.Any(char.IsDigit) && !input.Any(char.IsPunctuation);
        protected static bool NumbersArePositive(decimal input) => input > 0;
        protected static bool NumbersLettersAndWhitespaceOnlyNotJustWhiteSpaceOrNumbers(string? input)
        {
            return !string.IsNullOrEmpty(input) && input.All(x => x == char.Parse(" ") || char.IsLetterOrDigit(x)) && !input.All(char.IsWhiteSpace) && !input.All(char.IsDigit);
        }
        protected static Func<string, bool> ShouldBeLessThanOrEqualTo(int length)
        {
            return x => !string.IsNullOrEmpty(x) && x.Length <= length;
        }
        protected static bool MustBeBefore(DateTime? date, DateTime? dateToBeBefore)
        {
            return date is not null && dateToBeBefore is not null && date < dateToBeBefore;
        }
    }
}
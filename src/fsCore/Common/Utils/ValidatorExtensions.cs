using FluentValidation;

namespace Common.Utils
{
    public static class ValidatorExtensions
    {
        public static IValidator<IEnumerable<T>> CreateEnumerableValidator<T>(this IValidator<T> itemValidator) => new EnumerableValidator<T>(itemValidator);
    }
}
using FluentValidation;

namespace Common.Utils
{
    public class EnumerableValidator<T> : AbstractValidator<IEnumerable<T>>
    {
        public EnumerableValidator(IValidator<T> itemValidator)
        {
            RuleForEach(x => x).SetValidator(itemValidator);
        }

    }
}
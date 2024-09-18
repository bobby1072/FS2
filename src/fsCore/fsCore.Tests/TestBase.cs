using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace fsCore.Tests
{
    public abstract class TestBase
    {
        protected static readonly Fixture _fixture = new();
        protected static void SetupValidator<T>(Mock<IValidator<T>> validator) where T : class
        {
            validator
                .Setup(x => x.Validate(It.IsAny<ValidationContext<T>>()))
                .Returns(new ValidationResult());
        }
    }
}
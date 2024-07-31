using FluentValidation;

namespace Common.Models.Validators
{
    public class FourPointGeoAreaValidator : AbstractValidator<FourPointGeoArea>, IValidator<FourPointGeoArea>
    {
        public FourPointGeoAreaValidator()
        {
            RuleFor(x => x).Must(RectangleCorrectlyFormed).WithMessage(LiveMatchConstants.FourPointAreaIncorrectlyFormed);
        }
        private static bool RectangleCorrectlyFormed(FourPointGeoArea area)
        {
            return area.TopLeft.Latitude > area.BottomRight.Latitude && area.TopLeft.Longitude < area.BottomRight.Longitude;
        }
    }
}
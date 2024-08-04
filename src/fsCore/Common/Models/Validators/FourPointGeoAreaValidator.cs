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
            return area.TopLeft.Latitude > area.BottomLeft.Latitude &&
                   area.TopLeft.Latitude > area.TopRight.Latitude &&
                   area.BottomLeft.Latitude < area.BottomRight.Latitude &&
                   area.TopRight.Latitude < area.BottomRight.Latitude &&
                   area.TopLeft.Longitude < area.TopRight.Longitude &&
                   area.TopLeft.Longitude < area.BottomLeft.Longitude &&
                   area.TopRight.Longitude > area.BottomRight.Longitude &&
                   area.BottomLeft.Longitude < area.BottomRight.Longitude;
        }
    }
}
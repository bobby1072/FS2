using FluentValidation;
using fsCore.Common.Misc;
using fsCore.Common.Models;

namespace fsCore.Common.Models.Validators
{
    public class InAreaLiveMatchCatchRuleValidator : BaseValidator<InAreaLiveMatchCatchRule>
    {
        public InAreaLiveMatchCatchRuleValidator()
        {
            RuleFor(x => x.FourPointGeoAreas).Must(RectanglesCorrectlyFormed).WithMessage(LiveMatchConstants.FourPointAreaIncorrectlyFormed);
        }
        private static bool RectanglesCorrectlyFormed(IList<FourPointGeoArea> areas)
        {
            return areas.All(x => x.TopLeft.Latitude > x.BottomLeft.Latitude &&
                   x.TopLeft.Latitude > x.TopRight.Latitude &&
                   x.BottomLeft.Latitude < x.BottomRight.Latitude &&
                   x.TopRight.Latitude < x.BottomRight.Latitude &&
                   x.TopLeft.Longitude < x.TopRight.Longitude &&
                   x.TopLeft.Longitude < x.BottomLeft.Longitude &&
                   x.TopRight.Longitude > x.BottomRight.Longitude &&
                   x.BottomLeft.Longitude < x.BottomRight.Longitude);
        }
    }
}
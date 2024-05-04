using System.Text.Json.Serialization;
using Common.Models.Validators;
using FluentValidation;

namespace Common.Models.MiscModels
{
    public class LatLng
    {
        private static LatLngValidator _validator = new();
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
        [JsonConstructor]
        public LatLng(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
            _validator.ValidateAndThrow(this);
        }
    }
    public class LatLngValidator : BaseValidator<LatLng>
    {
        public LatLngValidator()
        {
            RuleFor(x => x.Latitude).Must(LatWithInRange).WithMessage(ErrorConstants.InvalidLatitudeAndLongitude);
            RuleFor(x => x.Longitude).Must(LngWithInRange).WithMessage(ErrorConstants.InvalidLatitudeAndLongitude);
        }
    }
}
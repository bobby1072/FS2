namespace Common.Models.MiscModels
{
    public class PartialGroupCatch
    {
        public string Species { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public PartialGroupCatch(string species, double latitude, double longitude)
        {
            Species = species;
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
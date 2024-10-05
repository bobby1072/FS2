namespace fsCore.ApiModels
{
    public record SaveCatchFormInput
    {
        public Guid? Id { get; set; }
        public Guid GroupId { get; set; }
        public string Species { get; set; }
        public double Weight { get; set; }
        public double Length { get; set; }
        public string? Description { get; set; }
        public string CaughtAt { get; set; }
        public IFormFile? CatchPhoto { get; set; }
        public string? CreatedAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? WorldFishTaxocode { get; set; }
    }
}
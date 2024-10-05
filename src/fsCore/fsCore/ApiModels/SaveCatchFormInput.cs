namespace fsCore.ApiModels
{
    public record SaveCatchFormInput
    {
        public Guid? Id { get; init; }
        public Guid GroupId { get; init; }
        public string Species { get; init; }
        public double Weight { get; init; }
        public double Length { get; init; }
        public string? Description { get; init; }
        public string CaughtAt { get; init; }
        public IFormFile? CatchPhoto { get; init; }
        public string? CreatedAt { get; init; }
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public string? WorldFishTaxocode { get; init; }
    }
}
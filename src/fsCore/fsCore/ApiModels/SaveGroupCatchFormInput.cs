namespace fsCore.ApiModels
{
    public record SaveGroupCatchFormInput
    {
        public Guid? Id { get; init; }
        public Guid GroupId { get; init; }
        public string Species { get; init; }
        public decimal Weight { get; init; }
        public decimal Length { get; init; }
        public string? Description { get; init; }
        public string CaughtAt { get; init; }
        public IFormFile? CatchPhoto { get; init; }
        public string? CreatedAt { get; init; }
        public decimal Latitude { get; init; }
        public decimal Longitude { get; init; }
        public string? WorldFishTaxocode { get; init; }
    }
}
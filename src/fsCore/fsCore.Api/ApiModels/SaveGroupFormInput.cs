namespace fsCore.Api.ApiModels
{
    public record SaveGroupFormInput
    {
        public IFormFile? Emblem { get; init; }
        public Guid? Id { get; init; }
        public Guid LeaderId { get; init; }
        public string Name { get; init; }
        public string? Description { get; init; }
        public string IsPublic { get; init; }
        public string IsListed { get; init; }
        public string CatchesPublic { get; init; }
        public string? CreatedAt { get; init; }

    }
}
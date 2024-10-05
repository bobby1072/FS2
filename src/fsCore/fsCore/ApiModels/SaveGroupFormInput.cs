namespace fsCore.ApiModels
{
    public record SaveGroupFormInput
    {
        public IFormFile? Emblem { get; set; }
        public Guid? Id { get; set; }
        public Guid LeaderId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string IsPublic { get; set; }
        public string IsListed { get; set; }
        public string CatchesPublic { get; set; }
        public string? CreatedAt { get; set; }

    }
}
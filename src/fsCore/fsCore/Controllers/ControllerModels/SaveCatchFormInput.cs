using System.Text.Json.Serialization;
using Common.Models;
using Common.Utils;

namespace fsCore.Controllers.ControllerModels
{
    public class SaveCatchFormInput
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
        public async Task<GroupCatch> ToGroupCatchAsync(Guid userId)
        {
            return new GroupCatch(
                userId,
                GroupId,
                Species,
                Weight,
                CaughtAt is not null ? DateTime.Parse(CaughtAt).ToUniversalTime() : DateTime.UtcNow,
                Length,
                Latitude,
                Longitude,
                Description,
                Id,
                CreatedAt is not null ? DateTime.Parse(CreatedAt).ToUniversalTime() : DateTime.UtcNow,
                CatchPhoto is not null ? await CatchPhoto.ToByteArrayAsync(null, null, 1) : null,
                null,
                null,
                WorldFishTaxocode,
                null
            );
        }
    }
}
using Common.Models;
using Common.Utils;

namespace fsCore.Controllers.ControllerModels
{
    public class SaveGroupFormInput
    {
        public IFormFile Emblem { get; set; }
        public Guid? Id { get; set; }
        public Guid LeaderId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string IsPublic { get; set; }
        public string IsListed { get; set; }
        public string CatchesPublic { get; set; }
        public string? CreatedAt { get; set; }
        public async Task<Group> ToGroupAsync()
        {
            return new Group(
                Name,
                Emblem is not null ? await Emblem.ToByteArrayAsync(720, 576, 0.5) : null,
                Description,
                Id is not null ? Id : null,
                CreatedAt is not null ? DateTime.Parse(CreatedAt).ToUniversalTime() : DateTime.UtcNow,
                bool.Parse(IsPublic),
                bool.Parse(IsListed),
                bool.Parse(CatchesPublic),
                LeaderId
            );
        }
    }
}
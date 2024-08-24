using Common.Models;

namespace fsCore.RequestModels
{
    public record GetSelfGroupsResponse
    {
        public ICollection<Group> Groups { get; set; }
        public ICollection<GroupMember> Memberships { get; set; }

    }
}
using Common.Models;

namespace fsCore.ApiModels
{
    public record GetSelfGroupsResponse
    {
        public ICollection<Group> Groups { get; set; }
        public ICollection<GroupMember> Memberships { get; set; }

    }
}
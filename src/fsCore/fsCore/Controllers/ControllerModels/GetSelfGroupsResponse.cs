using Common.Models;

namespace fsCore.Controllers.ControllerModels
{
    public class GetSelfGroupsResponse
    {
        public ICollection<Group> Groups { get; set; }
        public ICollection<GroupMember> Memberships { get; set; }

    }
}
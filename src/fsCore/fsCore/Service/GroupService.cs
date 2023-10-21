using System.Net;
using Common;
using Common.Dbinterfaces.Repository;
using Common.Models;
using Common.Permissions;
using Common.Utils;
using fsCore.Service.Interfaces;

namespace fsCore.Service
{
    internal class GroupService : BaseService<Group, IGroupRepository>, IGroupService
    {
        private readonly IGroupMemberRepository _groupMemberRepo;
        private readonly IGroupPositionRepository _groupPositionRepo;
        public GroupService(IGroupRepository repository,
        IGroupMemberRepository groupMemberRepo,
        IGroupPositionRepository groupPositionRepo) : base(repository)
        {
            _groupMemberRepo = groupMemberRepo;
            _groupPositionRepo = groupPositionRepo;
        }
        public async Task<ICollection<Group>> GetAllListedGroups()
        {
            var allGroups = await _repo.GetMany(true, "listed".ToPascalCase());
            return allGroups ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
        }
        public async Task<bool> IsUserInGroup(UserWithGroupPermissionSet currentUser, Guid group)
        {
            var foundGroup = await _repo.GetOne(group, "id".ToPascalCase()) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            return currentUser.Permissions.Can(PermissionConstants.BelongsTo, foundGroup);
        }
        public async Task<GroupMember> UserChangePositionInGroup(GroupMember newMember, UserWithGroupPermissionSet currentUser)
        {

        }
        public async Task<ICollection<GroupPosition>> GetAllPositionsForGroup(UserWithGroupPermissionSet currentUser, Guid groupId);
        public async Task<GroupMember> GetMembership(UserWithGroupPermissionSet currentUser, Guid groupId, bool includePosition = false, bool includeUser = false, bool includeGroup = false);
        public async Task<ICollection<GroupMember>> GetAllMemberships(User currentUser, bool includePosition = false, bool includeUser = false, bool includeGroup = false);
        public async Task<ICollection<GroupMember>> GetAllMemberships(UserWithGroupPermissionSet currentUser, bool includePosition = false, bool includeUser = false, bool includeGroup = false);
        public async Task<GroupMember> UserJoinGroup(GroupMember member, UserWithGroupPermissionSet currentUser);
        public async Task<GroupMember> UserLeaveGroup(UserWithGroupPermissionSet currentUser, Guid groupId);
        public async Task<ICollection<GroupMember>>? TryGetAllMemberships(User currentUser, bool includePosition = false, bool includeUser = false, bool includeGroup = false);
        public async Task<ICollection<GroupMember>> GetAllMembershipsForGroup(Guid groupId, UserWithGroupPermissionSet currentUser, bool includePosition = false, bool includeUser = false, bool includeGroup = false);
        public async Task<Group> CreateGroup(Group group, UserWithGroupPermissionSet currentUser);
        public async Task<Group> UpdateGroup(Group group, UserWithGroupPermissionSet currentUser);
        public async Task<Group> DeleteGroup(Group group, UserWithGroupPermissionSet currentUser);
    }
}
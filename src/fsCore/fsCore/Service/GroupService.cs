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
        public async Task<GroupMember> UserChangePositionInGroup(GroupMember newMember, UserWithGroupPermissionSet currentUser)
        {
            var foundMember = await _groupMemberRepo.GetOne(new Dictionary<string, object>
            {
                { "groupId".ToPascalCase(), newMember.GroupId },
                { "userEmail".ToPascalCase(), newMember.UserEmail }
            }, new List<string> { nameof(Group), nameof(User) }) ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
            if (foundMember.Group is null) throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (!currentUser.Permissions.Can(PermissionConstants.Manage, foundMember.Group, "Members") &&
              !currentUser.HasGlobalGroupManagePermissions(foundMember.Group))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _groupMemberRepo.Update(new List<GroupMember> { newMember }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);

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
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
        private static readonly Type _groupMemberType = typeof(GroupMember);
        private static readonly Type _groupType = typeof(Group);
        private static readonly Type _userType = typeof(User);
        private static readonly Type _groupPositionType = typeof(GroupPosition);
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
            var allGroups = await _repo.GetMany(true, _groupType.GetProperty("listed".ToPascalCase())?.Name ?? throw new Exception());
            return allGroups ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
        }
        private ICollection<string>? _produceRelationsList(bool includePosition = false, bool includeUser = false, bool includeGroup = false)
        {
            return _produceRelationsList(new Dictionary<string, bool>
            {
                { _groupPositionType.Name, includePosition },
                { _userType.Name, includeUser },
                { _groupType.Name, includeGroup }
            });
        }
        public async Task<GroupMember> UserChangePositionInGroup(GroupMember newMember, UserWithGroupPermissionSet currentUser)
        {
            var foundMember = await _groupMemberRepo.GetOne(new Dictionary<string, object>
            {
                { _groupMemberType.GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception(), newMember.GroupId },
                { _groupMemberType.GetProperty("userEmail".ToPascalCase())?.Name ?? throw new Exception(), newMember.UserEmail }
            }, new List<string> { _groupType.Name, _userType.Name }) ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
            if (foundMember.Group is null) throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (!currentUser.Permissions.Can(PermissionConstants.Manage, foundMember.Group, _groupMemberType.Name) &&
              !currentUser.HasGlobalGroupManagePermissions(foundMember.Group))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _groupMemberRepo.Update(new List<GroupMember> { newMember }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);

        }
        public async Task<ICollection<GroupPosition>> GetAllPositionsForGroup(UserWithGroupPermissionSet currentUser, Guid groupId)
        {
            var foundPositions = await _groupPositionRepo.GetMany(groupId, _groupPositionType.GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception(), _produceRelationsList(false, false, true)) ?? throw new ApiException(ErrorConstants.NoGroupPositionsFound, HttpStatusCode.NotFound);
            if (!currentUser.Permissions.Can(PermissionConstants.BelongsTo, foundPositions.FirstOrDefault()!.Group!) &&
                !currentUser.HasGlobalGroupReadPermissions(foundPositions.FirstOrDefault()!.Group!))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return foundPositions;
        }
        public async Task<GroupMember> GetMembership(UserWithGroupPermissionSet currentUser, string targetUserEmail, Guid groupId, bool includePosition = false, bool includeUser = false, bool includeGroup = false)
        {
            var foundMember = await _groupMemberRepo.GetOne(
             new Dictionary<string, object>
             {
                 { _groupMemberType.GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception(), groupId },
                 { _groupMemberType.GetProperty("userEmail".ToPascalCase())?.Name ?? throw new Exception(), currentUser.Email }
             },
            _produceRelationsList(includePosition, includeUser, includeGroup)) ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
            var foundGroup = foundMember.Group ?? throw new ApiException(ErrorConstants.NoGroupPositionsFound, HttpStatusCode.NotFound);
            if (!currentUser.Permissions.Can(PermissionConstants.BelongsTo, foundGroup) &&
                !currentUser.Permissions.Can(PermissionConstants.Read, foundGroup, _groupMemberType.Name) &&
              !currentUser.HasGlobalGroupReadPermissions(foundGroup))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return foundMember;
        }
        public async Task<ICollection<GroupMember>> GetAllMemberships(UserWithGroupPermissionSet currentUser, string targetEmail, bool includePosition = false, bool includeUser = false, bool includeGroup = false)
        {
            var foundMembers = await _groupMemberRepo.GetMany(
             currentUser.Email,
             _groupMemberType.GetProperty("userEmail".ToPascalCase())?.Name ?? throw new Exception(),
             _produceRelationsList(includePosition, includeUser, includeGroup)) ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
            return foundMembers;
        }
        public async Task<GroupMember> UserJoinGroup(GroupMember member, UserWithGroupPermissionSet currentUser)
        {
            var foundGroup = await _repo.GetOne(member.GroupId, _groupMemberType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            var foundMember = await _groupMemberRepo.GetOne(new Dictionary<string, object>
            {
                { _groupMemberType.GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception(), member.GroupId },
                { _groupMemberType.GetProperty("userEmail".ToPascalCase())?.Name ?? throw new Exception(), member.UserEmail }
            });
            if (foundMember is not null)
            {
                throw new ApiException(ErrorConstants.UserAlreadyExists, HttpStatusCode.Conflict);
            }
            else if (!(member.UserEmail == currentUser.Email && foundGroup.Public) &&
                !currentUser.Permissions.Can(PermissionConstants.Manage, foundGroup, _groupMemberType.Name)
            && !currentUser.HasGlobalGroupManagePermissions(foundGroup))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _groupMemberRepo.Create(new List<GroupMember> { member }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntCreateGroupMember, HttpStatusCode.InternalServerError);
        }
        public async Task<GroupMember> UserLeaveGroup(UserWithGroupPermissionSet currentUser, string targetUsername, Guid groupId)
        {
            var foundMember = await _groupMemberRepo.GetOne(new Dictionary<string, object>
            {
                { _groupMemberType.GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception(), groupId },
                { _groupMemberType.GetProperty("userEmail".ToPascalCase())?.Name ?? throw new Exception(), targetUsername }
            }, _produceRelationsList(false, false, true)) ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
            if (foundMember.Group is null)
            {
                throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            }
            else if (!(targetUsername == currentUser.Email) &&
                !currentUser.Permissions.Can(PermissionConstants.Manage, foundMember.Group, _groupMemberType.Name)
            && !currentUser.HasGlobalGroupManagePermissions(foundMember.Group))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _groupMemberRepo.Delete(new List<GroupMember> { foundMember }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntCreateGroupMember, HttpStatusCode.InternalServerError);

        }
        public async Task<ICollection<GroupMember>?> TryGetAllMemberships(User currentUser, bool includePosition = false, bool includeUser = false, bool includeGroup = false)
        {
            return await _groupMemberRepo.GetMany(
             currentUser.Email,
             _groupMemberType.GetProperty("userEmail".ToPascalCase())?.Name ?? throw new Exception(),
             _produceRelationsList(includePosition, includeUser, includeGroup));
        }
        public async Task<ICollection<GroupMember>> GetAllMembershipsForGroup(Guid groupId, UserWithGroupPermissionSet currentUser, bool includePosition = false, bool includeUser = false, bool includeGroup = false)
        {

        }
        public async Task<Group> SaveGroup(Group group, UserWithGroupPermissionSet currentUser);
        public async Task<Group> DeleteGroup(Group group, UserWithGroupPermissionSet currentUser);
    }
}
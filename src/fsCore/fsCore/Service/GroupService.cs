using System.Net;
using Common;
using Common.Dbinterfaces.Repository;
using Common.Models;
using Common.Permissions;
using Common.Utils;
using fsCore.Service.Interfaces;
using Microsoft.VisualBasic;

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
        public async Task<int> GetGroupCount()
        {
            return await _repo.GetCount();
        }
        public async Task<ICollection<Group>> GetAllListedGroups(int startIndex, int count)
        {
            if (count > 5) throw new ApiException(ErrorConstants.TooManyRecordsRequested, HttpStatusCode.BadRequest);
            var allGroups = await _repo.GetMany(startIndex, count, true, _groupType.GetProperty("listed".ToPascalCase())?.Name ?? throw new Exception(), _groupType.GetProperty("CreatedAt".ToPascalCase())?.Name ?? throw new Exception());
            return allGroups ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
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
            if (foundMember.Validate(newMember) is false) throw new ApiException(ErrorConstants.NotAllowedToEditThoseFields, HttpStatusCode.BadRequest);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundMember.Group, _groupMemberType.Name))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _groupMemberRepo.Update(new List<GroupMember> { newMember }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);

        }
        public async Task<ICollection<GroupPosition>> GetAllPositionsForGroup(UserWithGroupPermissionSet currentUser, Guid groupId)
        {
            var foundPositions = await _groupPositionRepo.GetMany(groupId, _groupPositionType.GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception(), _produceRelationsList(false, false, true)) ?? throw new ApiException(ErrorConstants.NoGroupPositionsFound, HttpStatusCode.NotFound);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.BelongsTo, foundPositions.FirstOrDefault()!.Group!))
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
            if (!currentUser.GroupPermissions.Can(PermissionConstants.BelongsTo, foundGroup) &&
                !currentUser.GroupPermissions.Can(PermissionConstants.Read, foundGroup, _groupMemberType.Name))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return foundMember;
        }
        public async Task<ICollection<GroupMember>> GetAllMemberships(UserWithGroupPermissionSet currentUser, string targetEmail, bool includePosition = false, bool includeUser = false, bool includeGroup = false)
        {
            var foundMembers = await _groupMemberRepo.GetMany(
             targetEmail,
             _groupMemberType.GetProperty("userEmail".ToPascalCase())?.Name ?? throw new Exception(),
             _produceRelationsList(includePosition, includeUser, includeGroup)) ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Read, foundMembers.FirstOrDefault()!.Group!, _groupMemberType.Name))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return foundMembers;
        }
        public async Task<GroupMember> UserJoinGroup(GroupMember member, UserWithGroupPermissionSet currentUser)
        {
            var foundGroup = _repo.GetOne(member.GroupId, _groupMemberType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            var foundMember = _groupMemberRepo.GetOne(new Dictionary<string, object>
            {
                { _groupMemberType.GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception(), member.GroupId },
                { _groupMemberType.GetProperty("userEmail".ToPascalCase())?.Name ?? throw new Exception(), member.UserEmail }
            });
            await Task.WhenAll(foundGroup, foundMember);
            if (foundMember.Result is not null)
            {
                throw new ApiException(ErrorConstants.UserAlreadyExists, HttpStatusCode.Conflict);
            }
            else if (!(member.UserEmail == currentUser.Email && foundGroup.Result.Public) &&
                !currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundGroup.Result, _groupMemberType.Name))
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
                !currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundMember.Group, _groupMemberType.Name))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _groupMemberRepo.Delete(new List<GroupMember> { foundMember }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntCreateGroupMember, HttpStatusCode.InternalServerError);

        }
        public async Task<ICollection<GroupMember>> GetAllMembershipsForGroup(Guid groupId, UserWithGroupPermissionSet currentUser, bool includePosition = false, bool includeUser = false)
        {
            var foundGroupMembers = await _groupMemberRepo.GetMany(
                groupId,
                _groupMemberType.GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception(),
                _produceRelationsList(includePosition, includeUser, true)) ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound
            );
            var foundGroup = foundGroupMembers.FirstOrDefault(x => x.Group is not null)?.Group;
            if (foundGroup is null)
            {
                throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            }
            else if (
                !currentUser.GroupPermissions.Can(PermissionConstants.Read, foundGroup!, _groupMemberType.Name))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return foundGroupMembers;
        }
        public async Task<Group> SaveGroup(Group group, UserWithGroupPermissionSet currentUser)
        {
            if (group.Id is not null)
            {
                var foundGroup = await _repo.GetOne(group.Id, _groupType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
                if (foundGroup.Validate(group) is false)
                {
                    throw new ApiException(ErrorConstants.NotAllowedToEditThoseFields, HttpStatusCode.BadRequest);
                }
                if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundGroup))
                {
                    throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
                }
                return (await _repo.Update(new List<Group> { group }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);
            }
            else
            {
                var newGroup = (await _repo.Create(new List<Group> { group.ApplyDefaults(currentUser.Email) }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);
                return newGroup;
            }

        }
        public async Task<Group> DeleteGroup(Guid group, UserWithGroupPermissionSet currentUser)
        {
            var foundGroup = await _repo.GetOne(group, _groupType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundGroup))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _repo.Delete(new List<Group> { foundGroup }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntDeleteGroup, HttpStatusCode.InternalServerError);
        }
        public async Task<GroupPosition> SavePosition(GroupPosition position, UserWithGroupPermissionSet currentUser)
        {
            var foundGroup = await _repo.GetOne(position.GroupId, _groupPositionType.GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundGroup))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            if (position.Id is null)
            {
                return (await _groupPositionRepo.Create(new List<GroupPosition> { position }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);
            }
            else
            {
                var foundPosition = await _groupPositionRepo.GetOne(position.Id, _groupPositionType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupPositionsFound, HttpStatusCode.NotFound);
                if (foundPosition.Validate(position) is false)
                {
                    throw new ApiException(ErrorConstants.NotAllowedToEditThoseFields, HttpStatusCode.BadRequest);
                }
                return (await _groupPositionRepo.Update(new List<GroupPosition> { position }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);

            }
        }
        public async Task<GroupPosition> DeletePosition(GroupPosition position, UserWithGroupPermissionSet currentUser)
        {
            var foundGroup = await _repo.GetOne(position.GroupId, _groupPositionType.GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundGroup))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _groupPositionRepo.Delete(new List<GroupPosition> { position }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntDeleteGroup, HttpStatusCode.InternalServerError);
        }
        public async Task<(ICollection<Group>, ICollection<GroupMember>)> GetAllGroupsAndMembershipsForUser(User currentUser)
        {
            var allMembers = _groupMemberRepo.GetMany(currentUser.Email, _groupMemberType.GetProperty("UserEmail".ToPascalCase())?.Name ?? throw new Exception(), _produceRelationsList(true, false, true));
            var allGroups = _repo.GetMany(currentUser.Email, _groupType.GetProperty("LeaderEmail".ToPascalCase())?.Name ?? throw new Exception());
            await Task.WhenAll(allMembers, allGroups);
            var finalGroupArray = allMembers.Result?.Select(x => x.Group).Union(allGroups.Result).ToHashSet();
            return (finalGroupArray ?? new HashSet<Group>(), allMembers.Result ?? new List<GroupMember>());
        }
        public async Task<(ICollection<Group>, ICollection<GroupMember>)> GetAllGroupsAndMembershipsForUserWithPagination(User currentUser, int startIndex, int count)
        {
            if (count > 5) throw new ApiException(ErrorConstants.TooManyRecordsRequested, HttpStatusCode.BadRequest);
            var allGroups = _repo.GetMany(startIndex, count, currentUser.Email, _groupType.GetProperty("LeaderEmail".ToPascalCase())?.Name ?? throw new Exception(), _groupType.GetProperty("CreatedAt".ToPascalCase())?.Name ?? throw new Exception());
            var allMembers = _groupMemberRepo.GetMany(currentUser.Email, _groupMemberType.GetProperty("UserEmail".ToPascalCase())?.Name ?? throw new Exception(), _produceRelationsList(true, false, false));
            await Task.WhenAll(allMembers, allGroups);
            var finalGroupArray = allMembers.Result?.Select(x => x.Group).Union(allGroups.Result).ToHashSet();
            return (finalGroupArray ?? new HashSet<Group>(), allMembers.Result ?? new List<GroupMember>());
        }
        public async Task<ICollection<Group>> GetAllSelfLeadGroups(User currentUser, int startIndex, int count)
        {
            if (count > 5) throw new ApiException(ErrorConstants.TooManyRecordsRequested, HttpStatusCode.BadRequest);
            var allGroups = await _repo.GetMany(startIndex, count, currentUser.Email, _groupType.GetProperty("LeaderEmail".ToPascalCase())?.Name ?? throw new Exception(), _groupType.GetProperty("CreatedAt".ToPascalCase())?.Name ?? throw new Exception());
            return allGroups ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
        }
        public async Task<Group> GetGroup(Guid groupId)
        {
            var foundGroup = await _repo.GetOne(groupId, _groupType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            return foundGroup;
        }
        public async Task<Group> GetFullGroup(Guid groupId, UserWithGroupPermissionSet currentUser)
        {
            var foundGroup = await _repo.GetOne(groupId, _groupType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception(), new List<string> { "Catches", "Positions", "Members", "Leader" }) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (!(currentUser.GroupPermissions.Can(PermissionConstants.Read, foundGroup) || (currentUser.GroupPermissions.Can(PermissionConstants.Read, foundGroup, _groupMemberType.Name) && currentUser.GroupPermissions.Can(PermissionConstants.Read, foundGroup, nameof(GroupCatch)) && currentUser.GroupPermissions.Can(PermissionConstants.BelongsTo, foundGroup))))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return foundGroup;
        }
    }
}
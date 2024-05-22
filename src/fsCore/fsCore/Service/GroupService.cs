using System.Net;
using Common;
using Common.DbInterfaces.Repository;
using Common.Models;
using Common.Models.Validators;
using Common.Permissions;
using Common.Utils;
using FluentValidation;
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
        private static readonly GroupValidator _groupValidator = new();
        private static readonly GroupPositionValidator _groupPositionValidator = new();
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
        public async Task<ICollection<Group>> SearchAllListedGroups(string groupNameString)
        {
            var allGroups = await _repo.SearchListedGroups(groupNameString);
            return allGroups ?? Array.Empty<Group>();
        }
        public async Task<ICollection<Group>> GetAllListedGroups(int startIndex, int count)
        {
            if (count > 5) throw new ApiException(ErrorConstants.TooManyRecordsRequested, HttpStatusCode.BadRequest);
            var allGroups = await _repo.GetMany(startIndex, count, true, _groupType.GetProperty("listed".ToPascalCase())?.Name ?? throw new Exception(), _groupType.GetProperty("CreatedAt".ToPascalCase())?.Name ?? throw new Exception());
            return allGroups ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
        }
        public async Task<Group> SaveGroup(Group group, UserWithGroupPermissionSet currentUser)
        {
            await _groupValidator.ValidateAndThrowAsync(group);
            if (group.Id is not null)
            {
                var foundGroup = await _repo.GetGroupWithoutEmblem(group.Id ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound)) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
                if (group.ValidateAgainstOriginal(foundGroup) is false)
                {
                    throw new ApiException(ErrorConstants.NotAllowedToEditThoseFields, HttpStatusCode.BadRequest);
                }
                if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundGroup))
                {
                    throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
                }
                return (await _repo.Update(new[] { group }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);
            }
            else
            {
                var newGroup = (await _repo.Create(new[] { group.ApplyDefaults(currentUser.Id) }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);
                return newGroup;
            }

        }
        public async Task<Group> DeleteGroup(Guid groupId, UserWithGroupPermissionSet currentUser)
        {
            var foundGroup = await _repo.GetOne(groupId, _groupType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundGroup))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _repo.Delete(new[] { foundGroup }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntDeleteGroup, HttpStatusCode.InternalServerError);
        }
        public async Task<GroupPosition> SavePosition(GroupPosition position, UserWithGroupPermissionSet currentUser)
        {
            await _groupPositionValidator.ValidateAndThrowAsync(position);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, position.GroupId))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            if (position.Id is null)
            {
                return (await _groupPositionRepo.Create(new[] { position }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);
            }
            else
            {
                var foundPosition = await _groupPositionRepo.GetOne(position.Id, _groupPositionType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupPositionsFound, HttpStatusCode.NotFound);
                if (foundPosition.ValidateAgainstOriginal(position) is false)
                {
                    throw new ApiException(ErrorConstants.NotAllowedToEditThoseFields, HttpStatusCode.BadRequest);
                }
                return (await _groupPositionRepo.Update(new[] { position }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);

            }
        }
        public async Task<GroupPosition> DeletePosition(Guid positionId, UserWithGroupPermissionSet currentUser)
        {
            var position = await _groupPositionRepo.GetOne(positionId, _groupPositionType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupPositionsFound, HttpStatusCode.NotFound);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, position.GroupId))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _groupPositionRepo.Delete(new[] { position }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntDeleteGroup, HttpStatusCode.InternalServerError);
        }
        public async Task<(ICollection<Group>, ICollection<GroupMember>)> GetAllGroupsAndMembershipsForUser(User currentUser)
        {
            var allMembersTask = _groupMemberRepo.GetMany(currentUser.Id, _groupMemberType.GetProperty("userId".ToPascalCase())?.Name ?? throw new Exception(), new string[] { "Group", "Position" });
            var allGroupsTask = _repo.ManyGroupWithoutEmblem(currentUser.Id ?? throw new Exception());
            await Task.WhenAll(allMembersTask, allGroupsTask);
            var allMembers = (await allMembersTask) ?? Array.Empty<GroupMember>();
            var allGroups = (await allGroupsTask) ?? Array.Empty<Group>();
            var finalGroupArray = allMembers.Select(x => x.Group).Union(allGroups).Where(x => x is not null).ToHashSet();
            return (finalGroupArray ?? new HashSet<Group>(), allMembers ?? Array.Empty<GroupMember>());
        }
        public async Task<ICollection<Group>> GetAllSelfLeadGroups(User currentUser, int startIndex, int count)
        {
            if (count > 5) throw new ApiException(ErrorConstants.TooManyRecordsRequested, HttpStatusCode.BadRequest);
            var allGroups = await _repo.GetMany(startIndex, count, currentUser.Id, _groupType.GetProperty("LeaderId".ToPascalCase())?.Name ?? throw new Exception(), _groupType.GetProperty("CreatedAt".ToPascalCase())?.Name ?? throw new Exception());
            return allGroups ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
        }
        public async Task<ICollection<Group>> GetAllGroupsForUser(User currentUser, int startIndex, int count)
        {
            if (count > 5) throw new ApiException(ErrorConstants.TooManyRecordsRequested, HttpStatusCode.BadRequest);
            var allMemberships = await _groupMemberRepo.GetFullMemberships(currentUser.Id ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound), count, startIndex);
            var allGroups = allMemberships?.Select(x => x.Group).ToArray();
            var nonNullGroupList = new Group[allGroups?.Length ?? 0];
            for (int i = 0; i < allGroups?.Length; i++)
            {
                var group = allGroups?[i];
                if (group is Group notNullGroup)
                {
                    nonNullGroupList[i] = notNullGroup;
                }
            }
            if (nonNullGroupList.Length < 1) throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            return nonNullGroupList;
        }
        public async Task<Group> GetGroupWithoutEmblemForInternalUse(Guid groupId)
        {
            var foundGroup = await _repo.GetGroupWithoutEmblem(groupId) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            return foundGroup;
        }
        public async Task<Group> GetGroupWithPositions(Guid groupId, UserWithGroupPermissionSet currentUser)
        {
            var foundGroup = await _repo.GetOne(groupId, _groupType.GetProperty("Id".ToPascalCase())?.Name ?? throw new Exception(), new string[] { "Positions", "Leader" }) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (foundGroup.Public is false && !currentUser.GroupPermissions.Can(PermissionConstants.BelongsTo, foundGroup))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            if (currentUser.Id != foundGroup.LeaderId)
            {
                foundGroup.Leader?.RemoveSensitive();
            }
            return foundGroup;
        }
        public async Task<ICollection<GroupMember>> GetGroupMembers(Guid groupId, UserWithGroupPermissionSet currentUser)
        {
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Read, groupId, nameof(GroupMember)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            var foundMembers = await _groupMemberRepo.GetMany(groupId, _groupMemberType.GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception(), new string[] { "User" });
            var finalMembersList = foundMembers?.ToArray() ?? Array.Empty<GroupMember>();
            for (var i = 0; i < finalMembersList.Length; i++)
            {
                var member = finalMembersList[i];
                if (member.User?.Email != currentUser.Email)
                {
                    member.User?.RemoveSensitive();
                }
            }
            return finalMembersList;
        }
        public async Task<GroupMember> SaveGroupMember(GroupMember groupMember, UserWithGroupPermissionSet currentUser)
        {
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, groupMember.GroupId, nameof(GroupMember)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            if (groupMember.Id is null)
            {
                return (await _groupMemberRepo.Create(new[] { groupMember }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);
            }
            else
            {
                var foundMember = await _groupMemberRepo.GetOne(groupMember.Id, _groupMemberType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
                if (foundMember.ValidateAgainstOriginal(groupMember) is false)
                {
                    throw new ApiException(ErrorConstants.NotAllowedToEditThoseFields, HttpStatusCode.BadRequest);
                }
                return (await _groupMemberRepo.Update(new[] { groupMember }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<GroupMember> DeleteGroupMember(Guid groupMemberId, UserWithGroupPermissionSet currentUser)
        {
            var foundGroupMember = await _groupMemberRepo.GetOne(groupMemberId, "id".ToPascalCase()) ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
            if (groupMemberId != currentUser.Id && !currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundGroupMember.GroupId!, nameof(GroupMember)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _groupMemberRepo.Delete(new[] { foundGroupMember }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntDeleteGroup, HttpStatusCode.InternalServerError);
        }
    }
}
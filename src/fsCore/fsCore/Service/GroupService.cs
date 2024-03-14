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
        private ICollection<string>? _produceRelationsList(bool includePosition = false, bool includeUser = false, bool includeGroup = false)
        {
            return _produceRelationsList(new Dictionary<string, bool>
            {
                { _groupPositionType.Name, includePosition },
                { _userType.Name, includeUser },
                { _groupType.Name, includeGroup }
            });
        }
        public async Task<Group> SaveGroup(Group group, UserWithGroupPermissionSet currentUser)
        {
            if (group.Id is not null)
            {
                var foundGroup = await _repo.GetOne(group.Id, _groupType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
                if (group.ValidateAgainstOriginal(foundGroup) is false)
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
                var newGroup = (await _repo.Create(new List<Group> { group.ApplyDefaults(currentUser.Id) }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);
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
            var foundGroup = await _repo.GetOne(position.GroupId, _groupType.GetProperty("Id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
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
                if (foundPosition.ValidateAgainstOriginal(position) is false)
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
            var allMembersTask = _groupMemberRepo.GetMany(currentUser.Id, _groupMemberType.GetProperty("userId".ToPascalCase())?.Name ?? throw new Exception(), new string[] { "Group", "Position" });
            var allGroupsTask = _repo.ManyGroupWithoutEmblem(currentUser.Id ?? throw new Exception());
            await Task.WhenAll(allMembersTask, allGroupsTask);
            var allMembers = (await allMembersTask) ?? Array.Empty<GroupMember>();
            var allGroups = (await allGroupsTask) ?? Array.Empty<Group>();
            var finalGroupArray = allMembers.Select(x => x.Group).Union(allGroups).ToHashSet();
            return (finalGroupArray ?? new HashSet<Group>(), allMembers ?? new List<GroupMember>());
        }
        public async Task<ICollection<Group>> GetAllSelfLeadGroups(User currentUser, int startIndex, int count)
        {
            if (count > 5) throw new ApiException(ErrorConstants.TooManyRecordsRequested, HttpStatusCode.BadRequest);
            var allGroups = await _repo.GetMany(startIndex, count, currentUser.Id, _groupType.GetProperty("LeaderId".ToPascalCase())?.Name ?? throw new Exception(), _groupType.GetProperty("CreatedAt".ToPascalCase())?.Name ?? throw new Exception());
            return allGroups ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
        }
        public async Task<Group> GetGroup(Guid groupId)
        {
            var foundGroup = await _repo.GetGroupWithoutEmblem(groupId) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            return foundGroup;
        }
        public async Task<Group> GetGroupWithPositions(Guid groupId, UserWithGroupPermissionSet currentUser)
        {
            var foundGroup = await _repo.GetOne(groupId, _groupType.GetProperty("Id".ToPascalCase())?.Name ?? throw new Exception(), new string[] { "Positions", "Leader" }) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (foundGroup.Public is false && (!currentUser.GroupPermissions.Can(PermissionConstants.Read, foundGroup) || !currentUser.GroupPermissions.Can(PermissionConstants.BelongsTo, foundGroup)))
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
            var foundGroupTask = _repo.GetGroupWithoutEmblem(groupId);
            var foundMembersTask = _groupMemberRepo.GetMany(groupId, _groupMemberType.GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception(), new string[] { "User" });
            await Task.WhenAll(foundGroupTask, foundMembersTask);
            var foundGroup = await foundGroupTask ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            var foundMembers = await foundMembersTask;
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Read, foundGroup, nameof(GroupMember)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            var finalMembersList = foundMembers ?? Array.Empty<GroupMember>();
            foreach (var member in finalMembersList)
            {
                if (member.User?.Email != currentUser.Email)
                {
                    member.User?.RemoveSensitive();
                }
            }
            return finalMembersList;
        }
        public async Task<GroupMember> SaveGroupMember(GroupMember groupMember, UserWithGroupPermissionSet currentUser)
        {
            var foundGroup = await _repo.GetGroupWithoutEmblem(groupMember.GroupId) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Manage, foundGroup, nameof(GroupMember)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            if (groupMember.Id is null)
            {
                return (await _groupMemberRepo.Create(new GroupMember[] { groupMember }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);
            }
            else
            {
                var foundMember = await _groupMemberRepo.GetOne(groupMember.Id, _groupMemberType.GetProperty("id".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoGroupMembersFound, HttpStatusCode.NotFound);
                if (foundMember.ValidateAgainstOriginal(groupMember) is false)
                {
                    throw new ApiException(ErrorConstants.NotAllowedToEditThoseFields, HttpStatusCode.BadRequest);
                }
                return (await _groupMemberRepo.Update(new GroupMember[] { groupMember }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveGroup, HttpStatusCode.InternalServerError);
            }
        }
    }
}
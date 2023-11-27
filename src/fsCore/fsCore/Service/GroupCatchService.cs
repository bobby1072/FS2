using System.Net;
using Common;
using Common.Dbinterfaces.Repository;
using Common.Models;
using Common.Permissions;
using Common.Utils;
using fsCore.Service.Interfaces;

namespace fsCore.Service
{
    internal class GroupCatchService : BaseService<GroupCatch, IGroupCatchRepository>, IGroupCatchService
    {
        private IGroupService _groupService;
        public GroupCatchService(IGroupCatchRepository repository, IGroupService groupServ) : base(repository)
        {
            _groupService = groupServ;
        }
        public async Task<ICollection<GroupCatch>> GetAllSelfCatches(User currentUser)
        {
            var foundCatches = await _repo.GetMany(currentUser.Email, typeof(User).GetProperty("email".ToPascalCase())?.Name ?? throw new Exception()) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            return foundCatches;
        }
        public async Task<ICollection<GroupCatch>> GetAllGroupCatches(UserWithGroupPermissionSet currentUser, Guid groupId)
        {
            var foundCatches = await _repo.GetMany(groupId, typeof(Group).GetProperty("id".ToPascalCase())?.Name ?? throw new Exception(), new List<String> { nameof(Group) }) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            var foundGroup = foundCatches.FirstOrDefault()?.Group ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (!currentUser.Permissions.Can(PermissionConstants.Read, foundGroup, nameof(GroupCatch)) && !currentUser.HasGlobalGroupReadPermissions(foundGroup))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return foundCatches;
        }
        public async Task<ICollection<GroupCatch>> GetAllCatchesAvailableToUser(UserWithGroupPermissionSet currentUser)
        {
            var foundMemberShips = await _groupService.GetAllMemberships(currentUser, currentUser.Email, false, false, true);
            if (!foundMemberShips.All(x => currentUser.Permissions.Can(PermissionConstants.Read, x.Group, nameof(GroupCatch)) || currentUser.HasGlobalGroupReadPermissions(x.Group)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return await _repo.GetMany(foundMemberShips.Select(x => x.Group.Id).ToList(), typeof(GroupCatch).GetProperty("groupId".ToPascalCase())?.Name ?? throw new Exception(), new List<String> { nameof(Group) }) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
        }
        public async Task<GroupCatch> SaveCatch(GroupCatch groupCatch, UserWithGroupPermissionSet currentUser)
        {
            var foundGroup = await _groupService.GetGroup(groupCatch.GroupId) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (groupCatch.UserEmail == currentUser.Email && !currentUser.Permissions.Can(PermissionConstants.BelongsTo, foundGroup) && !currentUser.HasGlobalGroupReadPermissions(foundGroup))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            if (groupCatch.UserEmail != currentUser.Email && !currentUser.Permissions.Can(PermissionConstants.Manage, foundGroup, nameof(GroupCatch)) && !currentUser.HasGlobalGroupManagePermissions(foundGroup))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            if (groupCatch.Id is null)
            {
                return (await _repo.Create(new List<GroupCatch> { groupCatch.ApplyDefaults() }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            }
            else
            {
                return (await _repo.Update(new List<GroupCatch> { groupCatch.ApplyDefaults() }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            }
        }
        public async Task<GroupCatch> DeleteCatch(GroupCatch groupCatch, UserWithGroupPermissionSet currentUser)
        {
            var foundGroup = await _groupService.GetGroup(groupCatch.GroupId) ?? throw new ApiException(ErrorConstants.NoGroupsFound, HttpStatusCode.NotFound);
            if (groupCatch.UserEmail == currentUser.Email && !currentUser.Permissions.Can(PermissionConstants.BelongsTo, foundGroup) && !currentUser.HasGlobalGroupReadPermissions(foundGroup))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            if (groupCatch.UserEmail != currentUser.Email && !currentUser.Permissions.Can(PermissionConstants.Manage, foundGroup, nameof(GroupCatch)) && !currentUser.HasGlobalGroupManagePermissions(foundGroup))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _repo.Delete(new List<GroupCatch> { groupCatch }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
        }
    }
}
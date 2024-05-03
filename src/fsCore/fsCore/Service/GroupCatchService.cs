using System.Net;
using Common;
using Common.DbInterfaces.Repository;
using Common.Models;
using Common.Models.MiscModels;
using Common.Permissions;
using fsCore.Service.Interfaces;

namespace fsCore.Service
{
    internal class GroupCatchService : BaseService<GroupCatch, IGroupCatchRepository>, IGroupCatchService
    {
        private readonly IWorldFishRepository _worldFishRepository;
        private readonly IGroupService _groupService;
        public GroupCatchService(IGroupCatchRepository groupCatchRepository, IWorldFishRepository worldFishRepo, IGroupService groupService) : base(groupCatchRepository)
        {
            _worldFishRepository = worldFishRepo;
            _groupService = groupService;
        }
        public async Task<GroupCatch> GetFullCatchById(Guid catchId, UserWithGroupPermissionSet currentUser)
        {
            var foundCatch = await _repo.GetOne(catchId) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            if (!currentUser.GroupPermissions.Can(PermissionConstants.Read, foundCatch.GroupId, nameof(GroupCatch)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return foundCatch;
        }
        public async Task<GroupCatch> DeleteGroupCatch(Guid id, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            var foundCatch = await _repo.GetOne(id) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            if (foundCatch.UserId != userWithGroupPermissionSet.Id && !userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Manage, foundCatch.GroupId, nameof(GroupCatch)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            else
            {
                if (!userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.BelongsTo, foundCatch.GroupId))
                {
                    throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
                }
            }
            return (await _repo.Delete(new[] { foundCatch }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.FailedToDeleteFish, HttpStatusCode.InternalServerError);
        }
        public async Task<ICollection<PartialGroupCatch>> GetAllPartialCatchesForGroup(Guid groupId, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            if (!userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Read, groupId, nameof(GroupCatch)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _repo.GetAllPartialCatchesForGroup(groupId))?.Select(x =>
            {
                x.User?.RemoveSensitive();
                return x;
            }).ToArray() ?? Array.Empty<PartialGroupCatch>();
        }
        public async Task<GroupCatch> SaveGroupCatch(GroupCatch groupCatch, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            if (groupCatch.UserId != userWithGroupPermissionSet.Id && !userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Manage, groupCatch.GroupId, nameof(GroupCatch)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            else
            {
                if (!userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.BelongsTo, groupCatch.GroupId))
                {
                    throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
                }
            }
            if (groupCatch.Id is Guid foundId)
            {
                var foundGroupCatch = await _repo.GetOne(foundId) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
                if (groupCatch.ValidateAgainstOriginal(foundGroupCatch) is false)
                {
                    throw new ApiException(ErrorConstants.NotAllowedToEditThoseFields, HttpStatusCode.BadRequest);
                }
                return (await _repo.Update(new[] { groupCatch }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveCatch, HttpStatusCode.InternalServerError);
            }
            else
            {
                return (await _repo.Create(new[] { groupCatch.ApplyDefaults() }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.CouldntSaveCatch, HttpStatusCode.InternalServerError);
            }
        }
        public async Task<GroupCatch> GetFullGroupCatchByLatAndLngWithAssociatedWorldFish(LatLng latLng, Guid groupId, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            if (!userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Read, groupId, nameof(GroupCatch)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            var groupCatch = await _repo.GetOne(latLng, groupId) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            groupCatch.User?.RemoveSensitive();
            return groupCatch;
        }
        public async Task<ICollection<PartialGroupCatch>> GetCatchesInSquareRange(LatLng bottomLeftLatLong, LatLng topRightLatLong, Guid groupId, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            if (!userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Read, groupId, nameof(GroupCatch)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _repo.GetCatchesInSquareRange(bottomLeftLatLong, topRightLatLong, groupId))?.Select(x =>
            {
                x.User?.RemoveSensitive();
                return x;
            }).ToArray() ?? Array.Empty<PartialGroupCatch>();
        }
    }
}
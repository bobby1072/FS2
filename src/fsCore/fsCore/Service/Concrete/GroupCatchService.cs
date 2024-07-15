using System.Net;
using System.Text.RegularExpressions;
using Common;
using Common.Models;
using Common.Models.MiscModels;
using Common.Models.Validators;
using Common.Permissions;
using FluentValidation;
using fsCore.Service.Abstract;
using Persistence.EntityFramework.Abstract.Repository;

namespace fsCore.Service.Concrete
{
    public class GroupCatchService : BaseService<GroupCatch, IGroupCatchRepository>, IGroupCatchService
    {
        private readonly IWorldFishRepository _worldFishRepository;
        private readonly IGroupService _groupService;
        private readonly IUserService _userService;
        private readonly IValidator<GroupCatch> _catchValidator;
        private readonly IValidator<GroupCatchComment> _commentValidator;
        private readonly IGroupCatchCommentRepository _commentRepo;
        public GroupCatchService(IGroupCatchRepository groupCatchRepository, IWorldFishRepository worldFishRepo, IGroupService groupService, IUserService userService, IGroupCatchCommentRepository commentRepo, IValidator<GroupCatchComment> commentValidator, IValidator<GroupCatch> catchValidator) : base(groupCatchRepository)
        {
            _catchValidator = catchValidator;
            _commentValidator = commentValidator;
            _worldFishRepository = worldFishRepo;
            _userService = userService;
            _commentRepo = commentRepo;
            _groupService = groupService;
        }
        public async Task<GroupCatch> GetFullCatchById(Guid catchId, UserWithGroupPermissionSet currentUser)
        {
            var foundCatch = await _repo.GetOne(catchId) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            if (foundCatch.Group?.CatchesPublic != true && !currentUser.GroupPermissions.Can(PermissionConstants.Read, foundCatch.GroupId, nameof(GroupCatch)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            foundCatch.User?.RemoveSensitive();
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
                if (!userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Read, foundCatch.GroupId, nameof(GroupCatch)))
                {
                    throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
                }
            }
            return (await _repo.Delete(new[] { foundCatch }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.FailedToDeleteFish, HttpStatusCode.InternalServerError);
        }
        public async Task<ICollection<PartialGroupCatch>> GetAllPartialCatchesForGroup(Guid groupId, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            var foundGroup = await _groupService.GetGroupWithoutEmblemForInternalUse(groupId);
            if (foundGroup.CatchesPublic == false && !userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Read, groupId, nameof(GroupCatch)))
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
            await _catchValidator.ValidateAndThrowAsync(groupCatch);
            if (groupCatch.UserId != userWithGroupPermissionSet.Id && !userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Manage, groupCatch.GroupId, nameof(GroupCatch)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
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
        public async Task<ICollection<PartialGroupCatch>> GetAllPartialCatchesForUser(Guid userId, UserWithGroupPermissionSet currentUser)
        {
            var allPartialCatches = await _repo.GetAllPartialCatchesForUser(userId) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            if (userId == currentUser.Id)
            {
                return allPartialCatches;
            }
            else
            {
                HashSet<Guid> groupIds = allPartialCatches.Select(x => x.GroupId).ToHashSet();
                var foundGroups = await _groupService.GetGroupsWithoutEmblemForInternalUse(groupIds);
                return allPartialCatches.Where(x => foundGroups.FirstOrDefault(y => y.Id == x.GroupId)?.CatchesPublic is true || currentUser.GroupPermissions.Can(PermissionConstants.Read, x.GroupId, nameof(GroupCatch))).Select(x =>
                {
                    x.User?.RemoveSensitive();
                    return x;
                }).ToArray();
            }
        }
        private async Task<ICollection<User>> FindTaggedUsersFromComment(string comment)
        {
            var foundTags = GroupCatchCommentUtils.RegexPattern.Matches(comment);
            if (foundTags is not MatchCollection confirmedMatches || confirmedMatches.Count < 1) return Array.Empty<User>();
            if (confirmedMatches.Count > GroupCatchCommentUtils.MaximumTags) throw new ValidationException("Too many users tagged in comment");
            var taggedUserIds = new HashSet<Guid> { };
            var uniqueTags = confirmedMatches.GroupBy(x => x.Value).Select(x => x.FirstOrDefault()).ToArray();
            foreach (var foundTag in uniqueTags)
            {
                if (foundTag is null) continue;
                var fixedTag = foundTag.ToString()?.Replace("@", "") ?? throw new ValidationException("Invalid users tagged");
                var taggedUserId = Guid.Parse(fixedTag);
                taggedUserIds.Add(taggedUserId);
            }
            var foundTaggedUsers = await _userService.GetUser(taggedUserIds) ?? throw new ValidationException("Invalid users tagged");
            if (foundTaggedUsers.Count != uniqueTags.Length) throw new ValidationException("Invalid tagged users");
            return foundTaggedUsers;
        }
        public async Task<GroupCatchComment> CommentOnCatch(GroupCatchComment groupCatchComment, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            await _commentValidator.ValidateAndThrowAsync(groupCatchComment);
            groupCatchComment = groupCatchComment.ApplyDefaults(userWithGroupPermissionSet.Id);
            var foundCatch = await _repo.GetOnePartial(groupCatchComment.GroupCatchId) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            if (groupCatchComment.UserId != userWithGroupPermissionSet.Id || !userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Read, foundCatch.GroupId, nameof(GroupCatch)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            if (groupCatchComment.Id is int foundId)
            {
                var foundComment = await _commentRepo.GetOne(foundId) ?? throw new ApiException(ErrorConstants.GroupCatchCommentNotFound, HttpStatusCode.NotFound);
                if (groupCatchComment.ValidateAgainstOriginal(foundComment) is false)
                {
                    throw new ApiException(ErrorConstants.NotAllowedToEditThoseFields, HttpStatusCode.BadRequest);
                }

                var taggedUsersJob = FindTaggedUsersFromComment(groupCatchComment.Comment);
                if (groupCatchComment.TaggedUsers?.Any() == true)
                {
                    var deleteTaggedUsersJob = _commentRepo.DeleteTaggedUsers(new[] { foundId });
                    await Task.WhenAll(deleteTaggedUsersJob, taggedUsersJob);
                }
                var taggedUsers = await taggedUsersJob;
                var updatedComment = (await _commentRepo.Update(new[] { groupCatchComment }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.GroupCatchCommentNotSaved, HttpStatusCode.InternalServerError);
                if (taggedUsers.Any()) await _commentRepo.CreateTaggedUsers(taggedUsers.Select(x => new GroupCatchCommentTaggedUsers(foundId, x.Id ?? throw new Exception())).ToArray());
                return updatedComment;
            }
            else
            {
                var taggedUsers = await FindTaggedUsersFromComment(groupCatchComment.Comment);
                var createdComment = (await _commentRepo.Create(new[] { groupCatchComment }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.GroupCatchCommentNotSaved, HttpStatusCode.InternalServerError);
                if (taggedUsers.Any()) await _commentRepo.CreateTaggedUsers(taggedUsers.Select(x => new GroupCatchCommentTaggedUsers(createdComment.Id ?? throw new Exception(), x.Id ?? throw new Exception())).ToArray());
                return createdComment;
            }
        }
        public async Task<GroupCatchComment> DeleteComment(int id, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            var foundComment = await _commentRepo.GetOne(id) ?? throw new ApiException(ErrorConstants.GroupCatchCommentNotFound, HttpStatusCode.NotFound);
            var foundCatch = await _repo.GetOnePartial(foundComment.GroupCatchId) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            if (foundComment.UserId != userWithGroupPermissionSet.Id && !userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Manage, foundCatch.GroupId, nameof(GroupCatch)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            return (await _commentRepo.Delete(new[] { foundComment }))?.FirstOrDefault() ?? throw new ApiException(ErrorConstants.GroupCatchCommentNotDeleted, HttpStatusCode.InternalServerError);
        }
        public async Task<ICollection<GroupCatchComment>> GetCommentsForCatch(Guid catchId, UserWithGroupPermissionSet userWithGroupPermissionSet)
        {
            var foundCatch = await _repo.GetOnePartial(catchId) ?? throw new ApiException(ErrorConstants.NoFishFound, HttpStatusCode.NotFound);
            if (!userWithGroupPermissionSet.GroupPermissions.Can(PermissionConstants.Read, foundCatch.GroupId, nameof(GroupCatch)))
            {
                throw new ApiException(ErrorConstants.DontHavePermission, HttpStatusCode.Forbidden);
            }
            var foundComments = await _commentRepo.GetAllForCatch(catchId) ?? Array.Empty<GroupCatchComment>();
            foundComments.RemoveSensitive();
            return foundComments;
        }
    }
}
using System.Net;
using Common.Misc;
using Common.Models;
using fsCore.Attributes;
using fsCore.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Services.Abstract;
namespace fsCore.Controllers
{
    [RequiredUser]
    [RequiredUserWithGroupPermissions]
    public class GroupCatchController : BaseController
    {
        private readonly IGroupCatchService _groupCatchService;
        public GroupCatchController(ILogger<GroupCatchController> logger, IGroupCatchService groupCatchService, ICachingService cachingService) : base(logger, cachingService)
        {
            _groupCatchService = groupCatchService;
        }
        [ProducesDefaultResponseType(typeof(int))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPost("CommentOnCatch")]
        public async Task<IActionResult> CommentOnCatch([FromBody] GroupCatchCommentInput groupCatchComment)
        {
            return Ok((await _groupCatchService.CommentOnCatch(groupCatchComment.ToGroupCatchComment(), await GetCurrentUserWithPermissionsAsync())).Id);
        }
        [ProducesDefaultResponseType(typeof(int))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("DeleteComment")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            return Ok((await _groupCatchService.DeleteComment(id, await GetCurrentUserWithPermissionsAsync())).Id);
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupCatchComment>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("GetCommentsForCatch")]
        public async Task<IActionResult> GetCommentsForCatch(Guid catchId)
        {
            return Ok(await _groupCatchService.GetCommentsForCatch(catchId, await GetCurrentUserWithPermissionsAsync()));
        }
        [ProducesDefaultResponseType(typeof(ICollection<PartialGroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("GetPartialCatchesForUser")]
        public async Task<IActionResult> GetPartialCatchesForUser(Guid userId)
        {
            return Ok(await _groupCatchService.GetAllPartialCatchesForUser(userId, await GetCurrentUserWithPermissionsAsync()));
        }
        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("DeleteGroupCatch")]
        public async Task<IActionResult> DeleteGroupCatch(Guid id)
        {
            return Ok((await _groupCatchService.DeleteGroupCatch(id, await GetCurrentUserWithPermissionsAsync())).Id);
        }
        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPost("SaveGroupCatch")]
        public async Task<IActionResult> SaveGroupCatch([FromForm] Guid? id, [FromForm] Guid groupId, [FromForm] string species, [FromForm] double weight, [FromForm] double length, [FromForm] string? description, [FromForm] string caughtAt, [FromForm] IFormFile? catchPhoto, [FromForm] string? createdAt, [FromForm] double latitude, [FromForm] double longitude, [FromForm] string? worldFishTaxocode)
        {
            var groupCatch = new SaveCatchFormInput
            {
                Id = id,
                GroupId = groupId,
                Species = species,
                Weight = weight,
                Length = length,
                Description = description,
                CaughtAt = caughtAt,
                CatchPhoto = catchPhoto,
                CreatedAt = createdAt,
                Latitude = latitude,
                Longitude = longitude,
                WorldFishTaxocode = worldFishTaxocode
            };
            var currentUser = await GetCurrentUserWithPermissionsAsync() ?? throw new ApiException(ErrorConstants.NoUserFound, HttpStatusCode.NotFound);
            return Ok((await _groupCatchService.SaveGroupCatch(await groupCatch.ToGroupCatchAsync(currentUser.Id ?? throw new Exception()), currentUser)).Id);
        }
        [ProducesDefaultResponseType(typeof(GroupCatch))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("GetFullCatchById")]
        public async Task<IActionResult> GetFullFishById(Guid catchId)
        {
            return Ok(await _groupCatchService.GetFullCatchById(catchId, await GetCurrentUserWithPermissionsAsync()));
        }
        [ProducesDefaultResponseType(typeof(ICollection<PartialGroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("GetCatchesInGroup")]
        public async Task<IActionResult> GetCatchesForGroup(Guid groupId)
        {
            return Ok(await _groupCatchService.GetAllPartialCatchesForGroup(groupId, await GetCurrentUserWithPermissionsAsync()));
        }
    }
}
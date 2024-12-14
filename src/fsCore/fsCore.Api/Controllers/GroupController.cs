using System.Net;
using fsCore.Api.ApiModels;
using fsCore.Api.Attributes;
using fsCore.Common.Misc;
using fsCore.Common.Models;
using fsCore.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace fsCore.Api.Controllers
{
    [RequiredUser]
    public class GroupController : BaseController
    {
        private readonly IGroupService _groupService;

        public GroupController(
            ILogger<GroupController> logger,
            IGroupService groupService,
            ICachingService cachingService
        )
            : base(logger, cachingService)
        {
            _groupService = groupService;
        }

        [ProducesDefaultResponseType(typeof(Group))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpGet("GetGroupWithPositions")]
        public async Task<IActionResult> GetGroupWithPositions(Guid groupId)
        {
            return Ok(
                await _groupService.GetGroupWithPositions(
                    groupId,
                    await GetCurrentUserWithPermissionsAsync()
                )
            );
        }

        [ProducesDefaultResponseType(typeof(ICollection<GroupMember>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpGet("GetGroupMembers")]
        public async Task<IActionResult> GetGroupMembers(Guid groupId)
        {
            return Ok(
                await _groupService.GetGroupMembers(
                    groupId,
                    await GetCurrentUserWithPermissionsAsync()
                )
            );
        }

        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions(true)]
        [HttpPost("SaveGroup")]
        public async Task<IActionResult> SaveGroup(
            [FromForm] IFormFile? emblem,
            [FromForm] Guid? id,
            [FromForm] string catchesPublic,
            [FromForm] Guid leaderId,
            [FromForm] string name,
            [FromForm] string? description,
            [FromForm] string isPublic,
            [FromForm] string isListed,
            [FromForm] string? createdAt
        )
        {
            var group = new SaveGroupFormInput
            {
                Emblem = emblem,
                Id = id,
                LeaderId = leaderId,
                Name = name,
                Description = description,
                CatchesPublic = catchesPublic,
                IsPublic = isPublic,
                IsListed = isListed,
                CreatedAt = createdAt,
            };
            var parsedGroup = await group.ToGroupAsync();
            var savedGroup = await _groupService.SaveGroup(
                parsedGroup,
                await GetCurrentUserWithPermissionsAsync()
            );
            return Ok(savedGroup.Id);
        }

        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions(true)]
        [HttpGet("DeleteGroup")]
        public async Task<IActionResult> DeleteGroup(Guid groupId)
        {
            return Ok(
                (
                    await _groupService.DeleteGroup(
                        groupId,
                        await GetCurrentUserWithPermissionsAsync()
                    )
                ).Id
            );
        }

        [ProducesDefaultResponseType(typeof(int))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions(true)]
        [HttpPost("SavePosition")]
        public async Task<IActionResult> SavePosition([FromBody] GroupPosition position)
        {
            return Ok(
                (
                    await _groupService.SavePosition(
                        position,
                        await GetCurrentUserWithPermissionsAsync()
                    )
                ).Id
            );
        }

        [ProducesDefaultResponseType(typeof(GroupPosition))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions(true)]
        [HttpGet("DeletePosition")]
        public async Task<IActionResult> DeletePosition(int positionId)
        {
            return Ok(
                await _groupService.DeletePosition(
                    positionId,
                    await GetCurrentUserWithPermissionsAsync()
                )
            );
        }

        [ProducesDefaultResponseType(typeof(ICollection<GroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpGet("GetAllListedGroups")]
        public async Task<IActionResult> ListedGroupsWithIndex(int startIndex, int count)
        {
            return Ok(await _groupService.GetAllListedGroups(startIndex, count));
        }

        [ProducesDefaultResponseType(typeof(ICollection<GroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpGet("SearchAllListedGroups")]
        public async Task<IActionResult> SearchAllListedGroups(string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                throw new ApiException(ErrorConstants.BadUrlParamsGiven, HttpStatusCode.BadRequest);
            }
            return Ok(await _groupService.SearchAllListedGroups(groupName));
        }

        [ProducesDefaultResponseType(typeof(int))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpGet("GetGroupCount")]
        public async Task<IActionResult> GetGroupCount()
        {
            return Ok(await _groupService.GetGroupCount());
        }

        [ProducesDefaultResponseType(typeof(ICollection<Group>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpGet("GetSelfGroups")]
        public async Task<IActionResult> GetSelfGroups(int startIndex, int count)
        {
            return Ok(
                await _groupService.GetAllSelfLeadGroups(
                    await GetCurrentUserAsync(),
                    startIndex,
                    count
                )
            );
        }

        [ProducesDefaultResponseType(typeof(ICollection<Group>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpGet("GetUsersGroups")]
        public async Task<IActionResult> GetUsersGroups(int startIndex, int count)
        {
            return Ok(
                await _groupService.GetAllGroupsForUser(
                    await GetCurrentUserAsync(),
                    startIndex,
                    count
                )
            );
        }

        [ProducesDefaultResponseType(typeof(int))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpPost("SaveGroupMember")]
        public async Task<IActionResult> SaveGroupMember([FromBody] GroupMember groupMember)
        {
            return Ok(
                (
                    await _groupService.SaveGroupMember(
                        groupMember,
                        await GetCurrentUserWithPermissionsAsync()
                    )
                ).Id
            );
        }

        [ProducesDefaultResponseType(typeof(int))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpGet("DeleteGroupMember")]
        public async Task<IActionResult> DeleteGroupMember(int groupMemberId)
        {
            return Ok(
                (
                    await _groupService.DeleteGroupMember(
                        groupMemberId,
                        await GetCurrentUserWithPermissionsAsync()
                    )
                ).Id
            );
        }
    }
}

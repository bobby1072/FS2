using System.Net;
using Common;
using Common.Models;
using fsCore.Controllers.Attributes;
using fsCore.Controllers.ControllerModels;
using fsCore.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
namespace fsCore.Controllers
{
    [RequiredUser]
    public class GroupController : BaseController
    {
        private readonly IGroupService _groupService;
        public GroupController(ILogger<GroupController> logger, IGroupService groupService) : base(logger)
        {
            _groupService = groupService;
        }
        [ProducesDefaultResponseType(typeof(Group))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpGet("GetGroupWithPositions")]
        public async Task<IActionResult> GetGroupWithPositions(Guid groupId)
        {
            return Ok(await _groupService.GetGroupWithPositions(groupId, GetCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupMember>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpGet("GetGroupMembers")]
        public async Task<IActionResult> GetGroupMembers(Guid groupId)
        {
            return Ok(await _groupService.GetGroupMembers(groupId, GetCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions(true)]
        [HttpPost("SaveGroup")]
        public async Task<IActionResult> SaveGroup([FromForm] IFormFile? emblem, [FromForm] Guid? id, [FromForm] string catchesPublic, [FromForm] Guid leaderId, [FromForm] string name, [FromForm] string? description, [FromForm] string isPublic, [FromForm] string isListed, [FromForm] string? createdAt)
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
                CreatedAt = createdAt
            };
            var parsedGroup = await group.ToGroupAsync();
            var savedGroup = await _groupService.SaveGroup(parsedGroup, GetCurrentUserWithPermissions());
            return Ok(savedGroup.Id);
        }
        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions(true)]
        [HttpGet("DeleteGroup")]
        public async Task<IActionResult> DeleteGroup(Guid groupId)
        {
            return Ok((await _groupService.DeleteGroup(groupId, GetCurrentUserWithPermissions())).Id);
        }
        [ProducesDefaultResponseType(typeof(int))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions(true)]
        [HttpPost("SavePosition")]
        public async Task<IActionResult> SavePosition([FromBody] GroupPosition position)
        {
            return Ok((await _groupService.SavePosition(position, GetCurrentUserWithPermissions())).Id);
        }
        [ProducesDefaultResponseType(typeof(GroupPosition))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions(true)]
        [HttpGet("DeletePosition")]
        public async Task<IActionResult> DeletePosition(int positionId)
        {
            return Ok(await _groupService.DeletePosition(positionId, GetCurrentUserWithPermissions()));
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
            return Ok(await _groupService.GetAllSelfLeadGroups(GetCurrentUser(), startIndex, count));
        }
        [ProducesDefaultResponseType(typeof(ICollection<Group>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpGet("GetUsersGroups")]
        public async Task<IActionResult> GetUsersGroups(int startIndex, int count)
        {
            return Ok(await _groupService.GetAllGroupsForUser(GetCurrentUser(), startIndex, count));
        }
        [ProducesDefaultResponseType(typeof(int))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpPost("SaveGroupMember")]
        public async Task<IActionResult> SaveGroupMember([FromBody] GroupMember groupMember)
        {
            return Ok((await _groupService.SaveGroupMember(groupMember, GetCurrentUserWithPermissions())).Id);
        }

        [ProducesDefaultResponseType(typeof(int))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithGroupPermissions]
        [HttpGet("DeleteGroupMember")]
        public async Task<IActionResult> DeleteGroupMember(int groupMemberId)
        {
            return Ok((await _groupService.DeleteGroupMember(groupMemberId, GetCurrentUserWithPermissions())).Id);
        }
    }
}
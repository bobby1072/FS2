using System.Net;
using Common;
using Common.Models;
using fsCore.Controllers.Attributes;
using fsCore.Controllers.ControllerModels;
using fsCore.Service.Interfaces;
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
        [RequiredUserWithPermissions]
        [HttpGet("GetGroupWithPositions")]
        public async Task<IActionResult> GetGroupWithPositions(Guid groupId)
        {
            return Ok(await _groupService.GetGroupWithPositions(groupId, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupMember>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("GetGroupMembers")]
        public async Task<IActionResult> GetGroupMembers(Guid groupId)
        {
            return Ok(await _groupService.GetGroupMembers(groupId, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions(true)]
        [HttpPost("SaveGroup")]
        public async Task<IActionResult> SaveGroup([FromForm] IFormFile emblem, [FromForm] Guid? id, [FromForm] Guid leaderId, [FromForm] string name, [FromForm] string? description, [FromForm] string isPublic, [FromForm] string isListed, [FromForm] string? createdAt)
        {
            var group = new SaveGroupFormInput
            {
                Emblem = emblem,
                Id = id,
                LeaderId = leaderId,
                Name = name,
                Description = description,
                IsPublic = isPublic,
                IsListed = isListed,
                CreatedAt = createdAt
            };
            var parsedGroup = await group.ToGroupAsync();
            var savedGroup = await _groupService.SaveGroup(parsedGroup, _getCurrentUserWithPermissions());
            return Ok(savedGroup.Id);
        }
        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions(true)]
        [HttpGet("DeleteGroup")]
        public async Task<IActionResult> DeleteGroup(Guid groupId)
        {
            return Ok((await _groupService.DeleteGroup(groupId, _getCurrentUserWithPermissions())).Id);
        }
        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions(true)]
        [HttpPost("SavePosition")]
        public async Task<IActionResult> SavePosition([FromBody] GroupPosition position)
        {
            return Ok((await _groupService.SavePosition(position, _getCurrentUserWithPermissions())).Id);
        }
        [ProducesDefaultResponseType(typeof(GroupPosition))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions(true)]
        [HttpGet("DeletePosition")]
        public async Task<IActionResult> DeletePosition(Guid positionId)
        {
            return Ok(await _groupService.DeletePosition(positionId, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("GetAllListedGroups")]
        public async Task<IActionResult> ListedGroupsWithIndex(int startIndex, int count)
        {
            return Ok(await _groupService.GetAllListedGroups(startIndex, count));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
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
        [RequiredUserWithPermissions]
        [HttpGet("GetGroupCount")]
        public async Task<IActionResult> GetGroupCount()
        {
            return Ok(await _groupService.GetGroupCount());
        }
        [ProducesDefaultResponseType(typeof(ICollection<Group>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("GetSelfGroups")]
        public async Task<IActionResult> GetSelfGroups(int startIndex, int count)
        {
            return Ok(await _groupService.GetAllSelfLeadGroups(_getCurrentUser(), startIndex, count));
        }
        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpPost("SaveGroupMember")]
        public async Task<IActionResult> SaveGroupMember([FromBody] GroupMember groupMember)
        {
            return Ok((await _groupService.SaveGroupMember(groupMember, _getCurrentUserWithPermissions())).Id);
        }
        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("DeleteGroupMember")]
        public async Task<IActionResult> DeleteGroupMember(Guid groupMemberId)
        {
            return Ok((await _groupService.DeleteGroupMember(groupMemberId, _getCurrentUserWithPermissions())).Id);
        }
    }
}
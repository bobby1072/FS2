using System.Net;
using Common.Models;
using fsCore.Controllers.Attributes;
using fsCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace fsCore.Controllers
{
    [RequiredUser]
    [RequiredUserWithPermissions]
    public class GroupController : BaseController
    {
        private readonly IGroupService _groupService;
        public GroupController(ILogger<BaseController> logger, IGroupService groupService) : base(logger)
        {
            _groupService = groupService;
        }
        [ProducesDefaultResponseType(typeof(ICollection<Common.Models.Group>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("GetAllListedGroups")]
        public async Task<IActionResult> GetAllListedGroupsRoute()
        {
            return Ok(await _groupService.GetAllListedGroups());
        }
        [ProducesDefaultResponseType(typeof(GroupMember))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPost("ChangePosition")]
        public async Task<IActionResult> ChangePosition([FromBody] GroupMember groupMember)
        {
            return Ok(await _groupService.UserChangePositionInGroup(groupMember, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupPosition>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("GetAllPositionsForGroup")]
        public async Task<IActionResult> GetAllPositionsForGroup(Guid groupId)
        {
            return Ok(await _groupService.GetAllPositionsForGroup(_getCurrentUserWithPermissions(), groupId));
        }
        [ProducesDefaultResponseType(typeof(GroupMember))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("GetMemberships")]
        public async Task<IActionResult> GetMembership(string targetUser, Guid groupId)
        {
            return Ok(await _groupService.GetMembership(_getCurrentUserWithPermissions(), targetUser, groupId));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupMember>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("GetAllMembershipsForUser")]
        public async Task<IActionResult> GetAllMemberships(string targetEmail)
        {
            return Ok(await _groupService.GetAllMemberships(_getCurrentUserWithPermissions(), targetEmail));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupMember>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("GetAllMembershipsForGroup")]
        public async Task<IActionResult> GetAllMembershipsForGroup(Guid groupId)
        {
            return Ok(await _groupService.GetAllMembershipsForGroup(groupId, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(GroupMember))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPost("JoinGroup")]
        public async Task<IActionResult> JoinGroup([FromBody] GroupMember groupMember)
        {
            return Ok(await _groupService.UserJoinGroup(groupMember, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(GroupMember))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("LeaveGroup")]
        public async Task<IActionResult> LeaveGroup(string targetUser, Guid groupId)
        {
            return Ok(await _groupService.UserLeaveGroup(_getCurrentUserWithPermissions(), targetUser, groupId));
        }
        [ProducesDefaultResponseType(typeof(Group))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPost("SaveGroup")]
        public async Task<IActionResult> SaveGroup([FromBody] Group group)
        {
            return Ok(await _groupService.SaveGroup(group, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(Group))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpPost("DeleteGroup")]
        public async Task<IActionResult> DeleteGroup([FromBody] Group group)
        {
            return Ok(await _groupService.DeleteGroup(group, _getCurrentUserWithPermissions()));
        }
    }
}
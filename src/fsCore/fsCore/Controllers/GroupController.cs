using System.Net;
using Common.Models;
using fsCore.Controllers.Attributes;
using fsCore.Controllers.ControllerModels;
using fsCore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [HttpGet("GetGroup")]
        public async Task<IActionResult> GetGroupWithMembers(Guid groupId)
        {
            return Ok(await _groupService.GetGroupAndMembers(groupId, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(GroupMember))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions(true)]
        [HttpPost("ChangePosition")]
        public async Task<IActionResult> ChangePosition([FromBody] GroupMember groupMember)
        {
            return Ok(await _groupService.UserChangePositionInGroup(groupMember, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupPosition>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("GetAllPositionsForGroup")]
        public async Task<IActionResult> GetAllPositionsForGroup(Guid groupId)
        {
            return Ok(await _groupService.GetAllPositionsForGroup(_getCurrentUserWithPermissions(), groupId));
        }
        [ProducesDefaultResponseType(typeof(GroupMember))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("GetMemberships")]
        public async Task<IActionResult> GetMembership(string targetUser, Guid groupId)
        {
            return Ok(await _groupService.GetMembership(_getCurrentUserWithPermissions(), targetUser, groupId));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupMember>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("GetAllMembershipsForUser")]
        public async Task<IActionResult> GetAllMemberships(string targetEmail)
        {
            return Ok(await _groupService.GetAllMemberships(_getCurrentUserWithPermissions(), targetEmail));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupMember>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("GetAllMembershipsForGroup")]
        public async Task<IActionResult> GetAllMembershipsForGroup(Guid groupId)
        {
            return Ok(await _groupService.GetAllMembershipsForGroup(groupId, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(GroupMember))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions(true)]
        [HttpPost("JoinGroup")]
        public async Task<IActionResult> JoinGroup([FromBody] GroupMember groupMember)
        {
            return Ok(await _groupService.UserJoinGroup(groupMember, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(GroupMember))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions(true)]
        [HttpGet("LeaveGroup")]
        public async Task<IActionResult> LeaveGroup(Guid targetUser, Guid groupId)
        {
            return Ok(await _groupService.UserLeaveGroup(_getCurrentUserWithPermissions(), targetUser, groupId));
        }
        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions(true)]
        [HttpPost("SaveGroup")]
        public async Task<IActionResult> SaveGroup([FromBody] SaveGroupInput group)
        {
            var savedGroup = await _groupService.SaveGroup(group.ToGroup(), _getCurrentUserWithPermissions());
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
        [ProducesDefaultResponseType(typeof(GroupPosition))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions(true)]
        [HttpPost("SavePosition")]
        public async Task<IActionResult> SavePosition([FromBody] GroupPosition position)
        {
            return Ok(await _groupService.SavePosition(position, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(GroupPosition))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions(true)]
        [HttpPost("DeletePosition")]
        public async Task<IActionResult> DeletePosition([FromBody] GroupPosition position)
        {
            return Ok(await _groupService.DeletePosition(position, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("GetAllListedGroups")]
        public async Task<IActionResult> ListedGroupsWithIndex(int startIndex, int count)
        {
            return Ok(await _groupService.GetAllListedGroups(startIndex, count));
        }
        [ProducesDefaultResponseType(typeof(int))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("GetGroupCount")]
        public async Task<IActionResult> GetGroupCount()
        {
            return Ok(await _groupService.GetGroupCount());
        }
        [ProducesDefaultResponseType(typeof(ICollection<Group>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [HttpGet("GetSelfGroups")]
        public async Task<IActionResult> GetSelfGroups(int startIndex, int count)
        {
            return Ok(await _groupService.GetAllSelfLeadGroups(_getCurrentUser(), startIndex, count));
        }
        // [ProducesDefaultResponseType(typeof(GetSelfGroupsResponse))]
        // [ProducesResponseType((int)HttpStatusCode.OK)]
        // [HttpGet("GetSelfGroups")]
        // public async Task<IActionResult> GetSelfGroups(int startIndex, int count)
        // {
        //     var (groups, memberships) = await _groupService.GetAllGroupsAndMembershipsForUserWithPagination(_getCurrentUser(), startIndex, count);
        //     var tempObj = new GetSelfGroupsResponse
        //     {
        //         Groups = groups,
        //         Memberships = memberships
        //     };
        //     return Ok(tempObj);
        // }
    }
}
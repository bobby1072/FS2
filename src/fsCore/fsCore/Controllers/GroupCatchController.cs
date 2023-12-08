using fsCore.Service.Interfaces;
using fsCore.Controllers.Attributes;
using Microsoft.AspNetCore.Mvc;
using Common.Models;
using System.Net;
namespace fsCore.Controllers
{

    [RequiredUser]
    public class GroupCatchController : BaseController
    {
        private IGroupCatchService _groupCatchService;
        public GroupCatchController(ILogger<GroupCatchController> logger, IGroupCatchService groupCatchService) : base(logger)
        {
            _groupCatchService = groupCatchService;
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("GetSelfCatches")]
        public async Task<IActionResult> GetSelfCatches()
        {
            return Ok(await _groupCatchService.GetAllSelfCatches(_getCurrentUser()));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("GetGroupCatches")]
        public async Task<IActionResult> GetGroupCatches(Guid groupId)
        {
            return Ok(await _groupCatchService.GetAllGroupCatches(_getCurrentUserWithPermissions(), groupId));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("GetAvailableCatches")]
        public async Task<IActionResult> GetAllCatchesForUser()
        {
            return Ok(await _groupCatchService.GetAllCatchesAvailableToUser(_getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("SaveCatch")]
        public async Task<IActionResult> SaveCatch([FromBody] GroupCatch groupCatch)
        {
            return Ok(await _groupCatchService.SaveCatch(groupCatch, _getCurrentUserWithPermissions()));
        }
        [ProducesDefaultResponseType(typeof(ICollection<GroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("DeleteCatch")]
        public async Task<IActionResult> DeleteCatch([FromBody] GroupCatch groupCatch)
        {
            return Ok(await _groupCatchService.DeleteCatch(groupCatch, _getCurrentUserWithPermissions()));
        }
    }
}
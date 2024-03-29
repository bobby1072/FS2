using System.Net;
using Common.Models;
using fsCore.Controllers.Attributes;
using fsCore.Controllers.ControllerModels;
using fsCore.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace fsCore.Controllers
{
    [RequiredUser]
    public class GroupCatchController : BaseController
    {
        private readonly IGroupCatchService _groupCatchService;
        public GroupCatchController(ILogger<GroupCatchController> logger, IGroupCatchService groupCatchService) : base(logger)
        {
            _groupCatchService = groupCatchService;
        }
        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpGet("DeleteGroupCatch")]
        public async Task<IActionResult> DeleteGroupCatch(Guid id)
        {
            return Ok((await _groupCatchService.DeleteGroupCatch(id, _getCurrentUserWithPermissions())).Id);
        }
        [ProducesDefaultResponseType(typeof(Guid))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpPost("SaveGroupCatch")]
        public async Task<IActionResult> SaveGroupCatch([FromBody] SaveCatchInput groupCatch)
        {
            var currentUser = _getCurrentUserWithPermissions();
            return Ok((await _groupCatchService.SaveGroupCatch(groupCatch.ToGroupCatch(currentUser.Id ?? throw new Exception()), currentUser)).Id);
        }
        [ProducesDefaultResponseType(typeof(GroupCatch))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpPost("GetFullFish")]
        public async Task<IActionResult> GetFullFish([FromBody] FullFishByLatLngInput input)
        {
            var (latLng, groupId) = input.BreakDown();
            return Ok(await _groupCatchService.GetFullGroupCatchByLatAndLngWithAssociatedWorldFish(latLng, groupId, _getCurrentUserWithPermissions()));
        }
    }
}
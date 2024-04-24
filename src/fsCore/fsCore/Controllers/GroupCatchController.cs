using System.Net;
using Common.Models;
using Common.Models.MiscModels;
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
            var currentUser = _getCurrentUserWithPermissions();
            return Ok((await _groupCatchService.SaveGroupCatch(await groupCatch.ToGroupCatchAsync(currentUser.Id ?? throw new Exception()), currentUser)).Id);
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
        [ProducesDefaultResponseType(typeof(ICollection<PartialGroupCatch>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [RequiredUserWithPermissions]
        [HttpPost("GetCatchesInSquareRange")]
        public async Task<IActionResult> GetCatchesInSquareRange([FromBody] SquareRangeCatchesInput input)
        {
            var (bottomLeftLatLong, topRightLatLong, groupId) = input.BreakDown();
            return Ok(await _groupCatchService.GetCatchesInSquareRange(bottomLeftLatLong, topRightLatLong, groupId, _getCurrentUserWithPermissions()));
        }
    }
}
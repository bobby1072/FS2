using System.Net;
using Common;
using Common.Models;
using fsCore.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fsCore.Controllers
{
    [Authorize]
    public class WorldFishController : BaseController
    {
        private readonly IWorldFishService _worldFishService;
        public WorldFishController(IWorldFishService worldFishService, ILogger<WorldFishController> logger) : base(logger)
        {
            _worldFishService = worldFishService;
        }
        [ProducesDefaultResponseType(typeof(ICollection<WorldFish>))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("FindSomeLike/{fishAnyName}")]
        public async Task<IActionResult> FindSomeLikeRoute(string fishAnyName)
        {
            if (string.IsNullOrEmpty(fishAnyName))
            {
                throw new ApiException(ErrorConstants.BadUrlParamsGiven, HttpStatusCode.NotFound);
            }
            return Ok(await _worldFishService.FindSomeLike(fishAnyName));
        }
        [ProducesDefaultResponseType(typeof(WorldFish))]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [HttpGet("FindOne/{fishProp}/{propertyName}")]
        public async Task<IActionResult> FindOneRoute(string fishProp, string propertyName)
        {
            if (string.IsNullOrEmpty(fishProp) || string.IsNullOrEmpty(propertyName))
            {
                throw new ApiException(ErrorConstants.BadUrlParamsGiven, HttpStatusCode.NotFound);
            }
            return Ok(await _worldFishService.FindOne(fishProp, propertyName));
        }
    }
}
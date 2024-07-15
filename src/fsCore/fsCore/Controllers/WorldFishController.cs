using System.Net;
using Common;
using Common.Models;
using fsCore.Service.Abstract;
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
        [HttpGet("FindSomeLike")]
        public async Task<IActionResult> FindSomeLikeRoute(string fishAnyName)
        {
            if (string.IsNullOrEmpty(fishAnyName))
            {
                throw new ApiException(ErrorConstants.BadUrlParamsGiven, HttpStatusCode.UnprocessableEntity);
            }
            return Ok(await _worldFishService.FindSomeLike(fishAnyName));
        }
    }
}
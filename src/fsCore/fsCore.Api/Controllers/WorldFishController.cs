using System.Net;
using fsCore.Common.Misc;
using fsCore.Common.Models;
using fsCore.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fsCore.Api.Controllers
{
    [Authorize]
    public class WorldFishController : BaseController
    {
        private readonly IWorldFishService _worldFishService;

        public WorldFishController(
            IWorldFishService worldFishService,
            ILogger<WorldFishController> logger,
            ICachingService cachingService
        )
            : base(logger, cachingService)
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
                throw new ApiException(
                    ErrorConstants.BadUrlParamsGiven,
                    HttpStatusCode.UnprocessableEntity
                );
            }
            return Ok(await _worldFishService.FindSomeLike(fishAnyName));
        }
    }
}

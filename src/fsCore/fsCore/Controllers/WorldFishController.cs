using System.Net;
using Common;
using Common.Models;
using fsCore.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fsCore.Controllers
{
    [AllowAnonymous]
    public class WorldFishController : BaseController
    {
        private readonly IWorldFishService _worldFishService;
        public WorldFishController(IWorldFishService worldFishService)
        {
            _worldFishService = worldFishService;
        }
        [ProducesDefaultResponseType(typeof(ICollection<WorldFish>))]
        [HttpGet("AllFish")]
        public async Task<IActionResult> GetAllFishRoute()
        {
            try
            {
                return Ok(await _worldFishService.AllFish());
            }
            catch (Exception e)
            {
                return await _routeErrorHandler(e);
            }
        }
        [ProducesDefaultResponseType(typeof(ICollection<WorldFish>))]
        [HttpGet("FindSomeLike/{fishAnyName}")]
        public async Task<IActionResult> FindSomeLikeRoute(string fishAnyName)
        {
            try
            {
                if (string.IsNullOrEmpty(fishAnyName))
                {
                    throw new ApiException(ErrorConstants.BadUrlParamsGiven, HttpStatusCode.NotFound);
                }
                return Ok(await _worldFishService.FindSomeLike(fishAnyName));
            }
            catch (Exception e)
            {
                return await _routeErrorHandler(e);
            }
        }
        [ProducesDefaultResponseType(typeof(WorldFish))]
        [HttpGet("FindOne/{fishProp}/{propertyName}")]
        public async Task<IActionResult> FindOneRoute(string fishProp, string propertyName)
        {
            try
            {
                if (string.IsNullOrEmpty(fishProp) || string.IsNullOrEmpty(propertyName))
                {
                    throw new ApiException(ErrorConstants.BadUrlParamsGiven, HttpStatusCode.NotFound);
                }
                return Ok(await _worldFishService.FindOne(fishProp, propertyName));
            }
            catch (Exception e)
            {
                return await _routeErrorHandler(e);
            }
        }
    }
}
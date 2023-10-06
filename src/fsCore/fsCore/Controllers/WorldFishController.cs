using System.ComponentModel;
using System.Net;
using Common.Models;
using fsCore.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

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
        public async Task<IActionResult> GetAllFish()
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
    }
}
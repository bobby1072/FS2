using Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace fsCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger _logger;
        public BaseController(ILogger logger)
        {
            _logger = logger;
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Common.Authentication;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace FsCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ClientConfigController : BaseController
    {
        private readonly ClientConfigSettings _clientConfigSettings;
        public ClientConfigController(IOptions<ClientConfigSettings> clientConfigSettings)
        {
            _clientConfigSettings = clientConfigSettings?.Value ?? throw new ArgumentNullException(nameof(clientConfigSettings));
        }
        [ProducesDefaultResponseType(typeof(ClientConfigurationResponse))]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(new ClientConfigurationResponse(
                    _clientConfigSettings.ApiHost,
                    _clientConfigSettings.AuthorityHost,
                    _clientConfigSettings.AuthorityScope,
                    _clientConfigSettings.AuthorityClientId));
            }
            catch (Exception e)
            {
                return await _routeErrorHandler(e);
            }
        }
    }

}

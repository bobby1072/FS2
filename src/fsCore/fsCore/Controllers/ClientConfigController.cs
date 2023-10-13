using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Common.Authentication;

namespace fsCore.Controllers
{
    [AllowAnonymous]
    internal class ClientConfigController : BaseController
    {
        private readonly ClientConfigSettings _clientConfigSettings;
        public ClientConfigController(IOptions<ClientConfigSettings> clientConfigSettings, ILogger<ClientConfigController> logger) : base(logger)
        {
            _clientConfigSettings = clientConfigSettings?.Value ?? throw new ArgumentNullException(nameof(clientConfigSettings));
        }
        [ProducesDefaultResponseType(typeof(ClientConfigurationResponse))]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(new ClientConfigurationResponse(
                _clientConfigSettings.ApiHost,
                _clientConfigSettings.AuthorityHost,
                _clientConfigSettings.AuthorityScope,
                _clientConfigSettings.AuthorityClientId));
        }
    }

}

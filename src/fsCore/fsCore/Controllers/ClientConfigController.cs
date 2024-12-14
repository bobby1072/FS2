using fsCore.Common.Authentication;
using fsCore.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace fsCore.Controllers
{
    [AllowAnonymous]
    public class ClientConfigController : BaseController
    {
        private readonly ClientConfigSettings _clientConfigSettings;

        public ClientConfigController(
            IOptions<ClientConfigSettings> clientConfigSettings,
            ICachingService cachingService,
            ILogger<ClientConfigController> logger
        )
            : base(logger, cachingService)
        {
            _clientConfigSettings =
                clientConfigSettings?.Value
                ?? throw new ArgumentNullException(nameof(clientConfigSettings));
        }

        [ProducesDefaultResponseType(typeof(ClientConfigurationResponse))]
        [HttpGet]
        public Task<IActionResult> Get()
        {
            return Task.FromResult(
                (IActionResult)Ok(
                    new ClientConfigurationResponse(
                        _clientConfigSettings.ApiHost,
                        _clientConfigSettings.AuthorityHost,
                        _clientConfigSettings.AuthorityClientId,
                        _clientConfigSettings.Scope
                    )
                )
            );
        }
    }
}

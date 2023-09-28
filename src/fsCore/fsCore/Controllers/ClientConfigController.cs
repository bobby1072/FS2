using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Common.Authentication;

namespace Core.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [AllowAnonymous]
    public class ClientConfigController : Controller
    {
        private readonly ClientConfigSettings _clientConfigSettings;
        public ClientConfigController(IOptions<ClientConfigSettings> clientConfigSettings)
        {
            _clientConfigSettings = clientConfigSettings?.Value ?? throw new ArgumentNullException(nameof(clientConfigSettings));
        }
        [HttpGet]
        public ClientConfigurationResponse Get()
        {
            return new ClientConfigurationResponse(
                _clientConfigSettings.ApiHost,
                _clientConfigSettings.AuthorityHost,
                _clientConfigSettings.AuthorityScope,
                _clientConfigSettings.AuthorityClientId);
        }
    }

}

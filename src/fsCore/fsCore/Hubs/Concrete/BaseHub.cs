using Microsoft.AspNetCore.SignalR;

namespace fsCore.Hubs.Concrete
{
    public abstract class BaseHub : Hub
    {
        private readonly ILogger _logger;
        public BaseHub(ILogger logger)
        {
            _logger = logger;
        }
    }
}
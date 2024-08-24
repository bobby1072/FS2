using Services.Abstract;

namespace fsCore.Controllers
{
    public class LiveMatchController : BaseController
    {
        private readonly ILiveMatchService _liveMatchService;
        public LiveMatchController(ILogger<LiveMatchController> logger, ICachingService cachingService, ILiveMatchService liveMatchService) : base(logger, cachingService)
        {
            _liveMatchService = liveMatchService;
        }
    }
}
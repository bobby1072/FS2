using Services.Abstract;

namespace Services.Concrete
{
    public class LiveMatchCachingService
    {
        private readonly ICachingService _cachingService;
        public LiveMatchCachingService(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }
    }
}
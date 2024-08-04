using Common.Models;
using Services.Abstract;
namespace Services.Concrete
{
    public class LiveMatchCachingService : ILiveMatchCachingService
    {
        private readonly ICachingService _cachingService;
        private const string _liveMatchKey = "match-";
        public LiveMatchCachingService(ICachingService cachingService)
        {
            _cachingService = cachingService;
        }
        public async Task<string> SetLiveMatch(LiveMatch liveMatch)
        {
            return await _cachingService.SetObject($"{_liveMatchKey}{liveMatch.Id}", liveMatch.ToCacheType());
        }
        public async Task<LiveMatch> GetLiveMatch(Guid matchId)
        {
            var cacheType = await _cachingService.GetObject<LiveMatchCacheType>($"{_liveMatchKey}{matchId}");
            return cacheType.ToRuntimeType();
        }
        public async Task<LiveMatch?> TryGetLiveMatch(Guid matchId)
        {
            var cacheType = await _cachingService.TryGetObject<LiveMatchCacheType>($"{_liveMatchKey}{matchId}");
            return cacheType?.ToRuntimeType();
        }
    }
}
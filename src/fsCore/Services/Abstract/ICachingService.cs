using Microsoft.Extensions.Caching.Distributed;
using Services.Concrete;

namespace Services.Abstract
{
    public interface ICachingService
    {
        Task<T> GetObject<T>(string key) where T : class;
        Task<T?> TryGetObject<T>(string key) where T : class;
        Task<string> SetObject<T>(string key, T value, DistributedCacheEntryOptions? options = null) where T : class;
        Task<string> SetObject<T>(string key, T value, CacheObjectTimeToLiveInSeconds timeToLive) where T : class;
    }
}
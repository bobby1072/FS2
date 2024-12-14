using fsCore.Services.Concrete;
using Microsoft.Extensions.Caching.Distributed;

namespace fsCore.Services.Abstract
{
    public interface ICachingService
    {
        Task<bool> TryRemoveObject(string key);
        Task<T> GetObject<T>(string key)
            where T : class;
        Task<T?> TryGetObject<T>(string key)
            where T : class;
        Task<string> SetObject<T>(string key, T value, DistributedCacheEntryOptions? options = null)
            where T : class;
        Task<string> SetObject<T>(string key, T value, CacheObjectTimeToLiveInSeconds timeToLive)
            where T : class;
    }
}

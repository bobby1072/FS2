using Microsoft.Extensions.Caching.Distributed;
using Services.Concrete;

namespace Services.Abstract
{
    public interface ICachingService
    {
        Task<T> GetObject<T>(string key);
        Task<T?> TryGetObject<T>(string key);
        Task<string> SetObject<T>(string key, T value, DistributedCacheEntryOptions? options = null);
        Task<string> SetObject<T>(string key, T value, CacheObjectTimeToLiveInSeconds timeToLive);
        Task<string?> TrySetObject<T>(string key, T value, CacheObjectTimeToLiveInSeconds timeToLive);
        Task<string?> TrySetObject<T>(string key, T value, DistributedCacheEntryOptions? options = null);
    }
}
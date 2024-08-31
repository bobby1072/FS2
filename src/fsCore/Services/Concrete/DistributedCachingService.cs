using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Services.Abstract;

namespace Services.Concrete
{
    public class DistributedCachingService : ICachingService
    {
        private readonly IDistributedCache _distributedCache;
        public DistributedCachingService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }
        public async Task<T> GetObject<T>(string key) where T : class
        {
            var foundValue = await _distributedCache.GetStringAsync(key) ?? throw new InvalidOperationException("Cannot find object with that key");
            return JsonSerializer.Deserialize<T>(foundValue) ?? throw new InvalidDataException("Cannot parse object");
        }
        public async Task<T?> TryGetObject<T>(string key) where T : class
        {
            try
            {
                return await GetObject<T>(key);
            }
            catch (Exception)
            {
                return default;
            }
        }
        public async Task<string> SetObject<T>(string key, T value, CacheObjectTimeToLiveInSeconds timeToLive) where T : class
        {
            return await SetObject(key, JsonSerializer.Serialize(value), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds((int)timeToLive)
            });
        }
        public async Task<string> SetObject<T>(string key, T value, DistributedCacheEntryOptions? options = null) where T : class
        {
            var serializedValue = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serializedValue, options ?? new DistributedCacheEntryOptions());
            return key;
        }
    }
    public enum CacheObjectTimeToLiveInSeconds
    {
        OneMinute = 60,
        FiveMinutes = 300,
        TenMinutes = 600,
        ThirtyMinutes = 1800,
        OneHour = 3600
    }
}
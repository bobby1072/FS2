using System.Text.Json;
using Common.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Services.Abstract;

namespace Services.Concrete
{
    public class CachingService : ICachingService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<CachingService> _logger;
        public CachingService(IDistributedCache distributedCache, ILogger<CachingService> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }
        public async Task<T> GetObject<T>(string key)
        {
            var foundValue = await _distributedCache.GetStringAsync(key) ?? throw new InvalidOperationException("Cannot find object with that key");
            return JsonSerializer.Deserialize<T>(foundValue) ?? throw new InvalidDataException("Cannot parse object");
        }
        public async Task<T?> TryGetObject<T>(string key)
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
        public async Task<string> SetObject<T>(string key, T value, CacheObjectTimeToLiveInSeconds timeToLive)
        {
            return await SetObject(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds((int)timeToLive)
            });
        }
        public async Task<string?> TrySetObject<T>(string key, T value, CacheObjectTimeToLiveInSeconds timeToLive)
        {
            try
            {
                return await SetObject(key, value, timeToLive);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> SetObject<T>(string key, T value, DistributedCacheEntryOptions? options = null)
        {
            var serializedValue = JsonSerializer.Serialize(value);
            if (value is BaseModel model)
            {
                model.RemoveSensitive();
                _logger.LogInformation("Setting object of type {ModelName} with values (sensitiveRemoved) {Model}", model.GetType().Name, model);
            }
            await _distributedCache.SetStringAsync(key, serializedValue, options ?? new DistributedCacheEntryOptions());
            return key;
        }
        public async Task<string?> TrySetObject<T>(string key, T value, DistributedCacheEntryOptions? options = null)
        {
            try
            {
                return await SetObject(key, value, options);
            }
            catch (Exception)
            {
                return null;
            }

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
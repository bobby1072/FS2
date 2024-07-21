using System.Text.Json;
using Common.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Services.Abstract;

namespace Services.Concrete
{
    public class DistributedCachingService : ICachingService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<DistributedCachingService> _logger;
        public DistributedCachingService(IDistributedCache distributedCache, ILogger<DistributedCachingService> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
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
        public async Task<string?> TrySetObject<T>(string key, T value, DistributedCacheEntryOptions? options = null) where T : class
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
        public async Task<string> SetObject<T>(string key, T value, CacheObjectTimeToLiveInSeconds timeToLive) where T : class
        {
            return await SetObject(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds((int)timeToLive)
            });
        }
        public async Task<string> SetObject<T>(string key, T value, DistributedCacheEntryOptions? options = null) where T : class
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
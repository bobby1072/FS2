using System.Text.Json;
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
        public async Task<string> SetObject<T>(string key, T value, DistributedCacheEntryOptions? options = null)
        {
            var serializedValue = JsonSerializer.Serialize(value);
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
}
using BBGPunchService.Infrastructure.Service.Handler.Interface;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace BBGPunchService.Infrastructure.Service.Handler.Implementation
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var data = await _cache.GetAsync(key);

            if (data == null)
                return default;

            var result = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(result);
        }


        public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options)
        {
            var data = JsonConvert.SerializeObject(value);
            var encodedData = Encoding.UTF8.GetBytes(data);
            await _cache.SetAsync(key, encodedData, options);
        }
        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration)
        {
            T? cacheValue = await GetAsync<T>(key);
            if (cacheValue != null && !cacheValue.Equals(0))
            {
                return cacheValue;
            }

            T? result = await factory();
            await SetAsync(key, result, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration });
            return result;
        }
    }
}

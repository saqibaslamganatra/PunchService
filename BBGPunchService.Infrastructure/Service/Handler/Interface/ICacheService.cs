using Microsoft.Extensions.Caching.Distributed;


namespace BBGPunchService.Infrastructure.Service.Handler.Interface
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options);
        Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, TimeSpan expiration);
    }

}

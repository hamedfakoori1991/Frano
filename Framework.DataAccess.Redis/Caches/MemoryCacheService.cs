using Microsoft.Extensions.Caching.Memory;

namespace Framework.DataAccess.Redis.Caches;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;

    public MemoryCacheService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public T? Get<T>(string key)
    {
        return _cache.TryGetValue(key, out T? value) ? value : default;
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = default)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            SlidingExpiration = expiration ?? TimeSpan.FromMinutes(5)
        };
        _cache.Set(key, value, cacheEntryOptions);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}

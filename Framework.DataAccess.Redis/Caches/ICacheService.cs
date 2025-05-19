namespace Framework.DataAccess.Redis.Caches;

public interface ICacheService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan? expiration = default);
    void Remove(string key);
}

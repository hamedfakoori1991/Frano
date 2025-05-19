using Framework.DataAccess.Redis.Caches;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Infrastructure.WebAPIs;

public static class CacheDependencyInjection
{
    public static IServiceCollection AddCacheService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICacheService, MemoryCacheService>();

        return services;
    }
}

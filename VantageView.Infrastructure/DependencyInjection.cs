using Microsoft.Extensions.Configuration;
using VantageView.Infrastructure.Configs;
using VantageView.Infrastructure.Repositories;
using Framework.DataAccess.Databases.MongoDb.Configs;

namespace Framework.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddVantageViewInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration, typeof(CrawlerConfigurationRepository).Assembly);
        services.AddScoped<IMongoDbSerializer, MongoDbSerializer>();
        return services;
    }
}



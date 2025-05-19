using Microsoft.Extensions.Configuration;
using VantageView.Application.CrawlerConfigurations.Commands;

namespace Framework.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddVantageViewApplication(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddApplication(configuration, typeof(SetupANewCrawlerConfigurationCommandHandler).Assembly);

        return services;
    }
}

using Serilog;
using Framework.Api.Extensions;
using Framework.Application.Behaviors;
using Framework.Tools.Messaging.SignalRServices.Contracts;
using VantageView.Contracts.CrawlerConfigurations;
using VantageView.RestApi.Validators;
using Framework.WebApi;
using Framework.Infrastructure.Logger;

namespace Framework.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddVantageViewApi(this IServiceCollection services, IConfiguration configuration, WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(SerilogHelper.Configure);

        services.AddWebApi(configuration, options =>
            options
                .SetAdditionalOpenApiSchemas(GetAdditionalSchemas().ToArray())
        .AddMappingFromAssemblyContaining(typeof(SetupNewCrawlerConfigurationRequest))
        .AddValidatorsFromAssemblyContaining(typeof(SetupNewCrawlerConfigurationValidator)));

        services.AddCustomSchemaLoggerConfigs(new CustomSchemaLogger());

        return services;
    }

    private static List<Type> GetAdditionalSchemas()
    {
        var additionalDtoSchemasTypes = new List<Type>
        {
        };
        var assembly = typeof(BaseScopeBehavior<,>).Assembly;
        foreach (var item in assembly.GetExportedTypes().Where(z => !z.IsAbstract))
        {
            var isAMessage = item.GetInterfaces().Any(z => z == typeof(IMessage));

            if (!isAMessage)
            {
                continue;
            }

            additionalDtoSchemasTypes.Add(item);
        }
        return additionalDtoSchemasTypes;
    }
}

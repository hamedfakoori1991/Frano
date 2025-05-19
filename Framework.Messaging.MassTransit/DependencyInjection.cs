using Framework.Messaging.MassTransit.MassTransitServices;
using Framework.Messaging.MassTransit.MassTransitServices.Filters;
using Framework.Messaging.MassTransit.MassTransitServices.Services;
using Framework.Messaging.MassTransit.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Framework.Messaging.MassTransit;

public static class DependencyInjection
{

    public static IServiceCollection AddMassTransitConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var kafkaSetting = configuration.GetSection(KafkaSettings.Name).Get<KafkaSettings>();
        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();

        var logger = loggerFactory.CreateLogger("DependencyInjection");

        if (!KafkaConnectionHelper.TryConnectKafka(kafkaSetting, logger))
        {
            logger.LogWarning($"kafka connection {kafkaSetting.ConnectionString} is not available");
            return services;
        }

        services.AddHostedService<MassTransitKafkaHostedService>();

        services.AddOptions<KafkaSettings>().Bind(configuration.GetSection(KafkaSettings.Name));
        services.AddScoped(typeof(PopulateScopeInfoConsumeFilter<>));
        services.AddScoped(typeof(AddHeaderFilter<>));

        return services;
    }

    public static string GetMteName(IConfiguration configuration) => configuration != null ? configuration["DatabaseMte"] ?? string.Empty : string.Empty;

    public static string GetConsumerGroupName(KafkaSettings kafkaSettings, IConfiguration configuration) => $"{kafkaSettings.GroupId}.{GetMteName(configuration)}";
}


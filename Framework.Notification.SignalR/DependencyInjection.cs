using System.Text.Json.Serialization;
using Framework.DataAccess.Redis.Settings;
using Framework.Tools.Messaging.SignalRServices.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Framework.Tools.Messaging;

public static class DependencyInjection
{
    public static IServiceCollection AddSignalR(this IServiceCollection services, IConfiguration configuration)
    {
        var redisSetting = configuration.GetSection(nameof(RedisDbSettings))
           .Get<RedisDbSettings>();

        services.AddSignalR().AddJsonProtocol(options =>
        {
            options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }).AddRedis(redisSetting);

        services.AddScoped<IMessagingService, SignalRPushMessageService>();
        services.AddSingleton<MessageHubStore>();

        return services;
    }

    public static void MapNotificationHub(this WebApplication? app)
    {
        app?.MapHub<MessageHub>(MessageHub.HubRoute);
    }

    public static ISignalRServerBuilder AddRedis(this ISignalRServerBuilder services, RedisDbSettings redisDbSettings)
    {
        if (!TryConnectRedis(redisDbSettings))
            return services;

        services.AddStackExchangeRedis(redisDbSettings?.ConnectionString, options =>
        {
            options.Configuration.ChannelPrefix = typeof(MessageHub).Name;
        });

        return services;
    }

    private static bool TryConnectRedis(RedisDbSettings redisDbSettings)
    {
        if (redisDbSettings == null || string.IsNullOrEmpty(redisDbSettings.ConnectionString))
            return false;

        var options = ConfigurationOptions.Parse(redisDbSettings.ConnectionString);
        options.AbortOnConnectFail = false; // Prevent exceptions on failed connection
        var connectionMultiplexer = ConnectionMultiplexer.Connect(options);

        if (!connectionMultiplexer.IsConnected)
            return false;

        connectionMultiplexer.Close();
        return true;
    }
}

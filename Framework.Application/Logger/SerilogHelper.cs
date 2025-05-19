using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Framework.Infrastructure.Logger;

public static class SerilogHelper
{
    public static Action<HostBuilderContext, IServiceProvider, LoggerConfiguration> Configure =>
        (context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces);
        };

    // The initial "bootstrap" logger is able to log errors during start-up. It's completely replaced by the
    // logger configured in `UseSerilog()` below, once configuration and dependency-injection have both been
    // set up successfully.
    public static void CreateBootstrapLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();
    }
}

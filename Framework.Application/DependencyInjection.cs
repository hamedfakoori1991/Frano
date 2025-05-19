using System.Reflection;
using Framework.Application.Behaviors;
using Framework.Tools.Messaging;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration, Assembly handlerAssembly)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(BaseScopeBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(BaseHandlerBehavior<,>));
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(handlerAssembly));

        services.AddSignalR(configuration);
        return services;
    }
}

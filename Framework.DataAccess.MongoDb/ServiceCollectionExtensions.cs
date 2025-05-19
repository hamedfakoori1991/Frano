using System.Reflection;
using Framework.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
namespace Framework.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, Assembly assembly)
    {
        foreach (var item in assembly.GetExportedTypes())
        {
            var itIsARepository = item.GetInterfaces()
                .Any(z => z.IsGenericType && typeof(IRepository<,>) == z.GetGenericTypeDefinition());

            if (!itIsARepository)
                continue;

            var service = item.GetInterfaces().FirstOrDefault(z => !z.IsGenericType);

            if (service == null)
                continue;

            services.AddScoped(service, item);

        }
        return services;
    }
}

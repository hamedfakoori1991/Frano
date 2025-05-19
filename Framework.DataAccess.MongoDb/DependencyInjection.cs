using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Framework.DataAccess.Databases.MongoDb.Configs;
using Framework.DataAccess.Databases.MongoDb.Services;
using Framework.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Framework.DataAccess.MongoDb.MongoDb.Services;
using System.Reflection;

namespace Framework.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Assembly Repository)
    {
        services.AddHttpContextAccessor();

        var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings))
            .Get<MongoDbSettings>();

        services.AddHealthChecks().AddMongoDb(
            name: "MongoDB",
            tags: ["dependencies", "exclude_from_hc"]);


        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
        });

        services.AddScoped(typeof(MongoDbSettings), z =>
        {
            var mte = configuration.GetSection("DatabaseMte").Value!;

            var service = z.GetService<IApplicationScopeInfo>();
            service.SetMte(mte); 
            return new MongoDbSettings { ConnectionString = mongoDbSettings.ConnectionString, Database = string.Format(mongoDbSettings.DatabaseNameFormat, service.Mte, service.Tenant) };
        });


        services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoDbSettings?.ConnectionString));

        services.AddScoped<IDbContextSeedData, MongoDbSeedData>();
        services.AddScoped<IDbContext, MongoDbContext>();

        services.AddRepositories(Repository);

        services.AddScoped<IDbContextSeedData, MongoDbSeedData>();

        services.AddLogging();

        services.AddScoped<IDbSessionManager, MongoDbSessionManager>();

        services.AddScoped<IdGenerator, MongoIdGenerator>();

        return services;
    }
}



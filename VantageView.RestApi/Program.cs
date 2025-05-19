using Serilog;
using Framework.Api;
using Framework.Application;
using Framework.Infrastructure;
using Framework.Tools.Messaging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Framework.WebApi.Authorizations;
using Framework.WebApi.Middlewares;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        {
            builder.Services
                .AddVantageViewInfrastructure(builder.Configuration)
                .AddVantageViewApi(builder.Configuration, builder)
                .AddVantageViewApplication(builder.Configuration);
        }

        var app = builder.Build();
        {
            app.UseExceptionHandlingMiddleware();
            app.UseSerilogRequestLogging();

            /*if (app.Environment.IsDevelopment())
            {

            }*/

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.EnableFilter();
                c.DefaultModelsExpandDepth(-1);
                c.DisplayRequestDuration();
            });

            app.UseForwardedHeaders();

            app.UseHealthChecks("/hc", new HealthCheckOptions
            {
                // Include only health checks that do not have the "exclude_from_hc" tag
                Predicate = check => !check.Tags.Contains("exclude_from_hc")
            });

            app.UseHealthCheckMiddleware();

            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMultiTenancyMiddleware();
            app.MapControllers()
                .RequireAuthorization(AuthorizationsConsts.TenantIsRequired);

            app.MapNotificationHub();
            app.Run();
        }
    }
}

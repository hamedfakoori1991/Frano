using Framework.WebApi.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;

namespace Framework.WebApi.Middlewares;
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        MessageResultExtensions.Environment = ((WebApplication)builder).Environment;
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }

    public static IApplicationBuilder UseMultiTenancyMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MultiTenancyMiddleware>();
    }

    public static IApplicationBuilder UseHealthCheckMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseHealthChecks("/health/dependencies", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("dependencies") || check.Tags.Contains("masstransit"),
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var response = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        key = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        exceptionMessage = e.Value.Exception?.Message,
                        exceptionDeatil = e.Value.Exception?.ToString()
                    })
                };
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }
        });
    }
}

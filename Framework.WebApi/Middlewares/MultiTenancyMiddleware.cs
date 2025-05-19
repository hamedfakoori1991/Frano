using Framework.Application.Interfaces;
using Framework.Application.Jobs.ExecutionPlan;
using Framework.Domain.Common;
using Framework.Infrastructure.Logger;
using Framework.WebApi.Authorizations;
using Framework.WebApi.Settings;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace Framework.WebApi.Middlewares;

public class MultiTenancyMiddleware
{
    private readonly RequestDelegate _next;

    public MultiTenancyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApplicationScopeInfo applicationScopeInfo, ApiConfigurationOption option)
    {
        var claimValue = context?.User?.FindFirst(AuthorizationsConsts.Tenant)?.Value;

        applicationScopeInfo.SetClaimsPrincipal(context?.User);
        applicationScopeInfo.SetTenant(claimValue);

        applicationScopeInfo.SetDefaultLanguage(GetDefaultLanguage(context));

        using (LogContext.Push(new UserInfoLogEnricher(applicationScopeInfo)))
        {
            await _next(context);
        }
        ;
    }


    private string GetDefaultLanguage(HttpContext context)
    {
        context.Request.Headers.TryGetValue(DomainConsts.LanguageColumnName, out var language);
        if (!string.IsNullOrEmpty(language))
            return language;

        return context?.User?.GetUserLanguage() ?? string.Empty;
    }
}

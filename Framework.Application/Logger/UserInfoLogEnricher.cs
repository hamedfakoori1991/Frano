using Framework.Application.Interfaces;
using Serilog.Core;
using Serilog.Events;

namespace Framework.Infrastructure.Logger;
public class UserInfoLogEnricher : ILogEventEnricher
{
    private readonly IApplicationScopeInfo _applicationScopeInfo;

    public UserInfoLogEnricher(IApplicationScopeInfo applicationScopeInfo)
    {
        _applicationScopeInfo = applicationScopeInfo;
    }

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent == null || _applicationScopeInfo == null)
            return;

        var user = new
        {
            UserId = _applicationScopeInfo.GetCurrentUserId(),
            _applicationScopeInfo.Tenant,

            //TODO: user roles 
        };

        var userIdProperty = propertyFactory.CreateProperty("UserInfo", user, true);
        logEvent.AddPropertyIfAbsent(userIdProperty);
    }
}



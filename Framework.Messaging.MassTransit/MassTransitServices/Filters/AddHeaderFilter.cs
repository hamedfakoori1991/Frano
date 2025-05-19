using Framework.Application.Interfaces;
using MassTransit;

namespace Framework.Messaging.MassTransit.MassTransitServices.Filters;

public class AddHeaderFilter<T> :
    IFilter<SendContext<T>> where T : class
{
    private readonly IApplicationScopeInfo _applicationScopeInfo;

    public AddHeaderFilter(IApplicationScopeInfo applicationScopeInfo)
    {
        _applicationScopeInfo = applicationScopeInfo;
    }

    public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        context.Headers.Set("tenant", _applicationScopeInfo.Tenant);
        context.Headers.Set("userId", _applicationScopeInfo.GetCurrentUserId());
        context.Headers.Set("userName", _applicationScopeInfo.GetCurrentUserName());
        await next.Send(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("AddHeaderFilter");
    }
}

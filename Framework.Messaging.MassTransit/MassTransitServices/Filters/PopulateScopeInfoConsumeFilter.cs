using Framework.Application.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Framework.Messaging.MassTransit.MassTransitServices.Filters;

public class PopulateScopeInfoConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly IApplicationScopeInfo _applicationScopeInfo;
    private readonly ILogger<PopulateScopeInfoConsumeFilter<T>> _logger;
    public PopulateScopeInfoConsumeFilter(IApplicationScopeInfo applicationScopeInfo, ILogger<PopulateScopeInfoConsumeFilter<T>> logger)
    {
        _applicationScopeInfo = applicationScopeInfo;
        _logger = logger;
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        try
        {

            if (context.Headers.TryGetHeader("tenant", out var tenant))
            {
                _applicationScopeInfo.SetTenant(tenant?.ToString());
            }
            else
            {
                _logger.LogWarning("tenant is missing");
            }

            if (context.Headers.TryGetHeader("userId", out var userId))
            {
                _applicationScopeInfo.SetUserId(userId?.ToString());
            }
            if (context.Headers.TryGetHeader("userName", out var userName))
            {
                _applicationScopeInfo.SetUserName(userName?.ToString());
            }

            var messageType = context.Message.GetType().Name;

            await next.Send(context);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex);
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("PopulateScopeInfoConsumeFilter");
    }

    private void AddLog(Guid? messageId, string messageType, string title, string tenant, string? currentUserId, string? currentUserName,
        string? templateId)
    {
        _logger.LogInformation(
            @$"{title}: {messageId}
            Headers:
            - Message Type: {messageType}
            - Tenant: {tenant}
            - UserId: {currentUserId}
            - UserName: {currentUserName}
            - TemplateId: {templateId}"
            );
    }
}

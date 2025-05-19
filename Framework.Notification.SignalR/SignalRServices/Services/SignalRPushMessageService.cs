using Framework.Tools.Messaging.SignalRServices.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Framework.Tools.Messaging.SignalRServices.Services;

public class SignalRPushMessageService : IMessagingService
{
    private readonly IHubContext<MessageHub, IMessageHub> _context;
    private readonly MessageHubStore _hubStore;

    public SignalRPushMessageService(IHubContext<MessageHub, IMessageHub> context, MessageHubStore hubStore)
    {
        _context = context;
        _hubStore = hubStore;
    }
    public async Task SendMessageToCallerAsync(IMessage message, string caller)
    {
        if (!string.IsNullOrEmpty(caller))
        {
            if (_hubStore.TryGetUserConnectionIds(caller, out var connectionIds))
            {
                foreach (var con in connectionIds)
                {
                    await _context.Clients.Client(con).MessageReceived(message);
                }
            }
        }
    }

    public async Task SendMessageToClientAsync(IMessage message, string connectionId)
    {
        if (!string.IsNullOrEmpty(connectionId))
            await _context.Clients.Client(connectionId).MessageReceived(message);
    }

    public async Task SendMessageToAllAsync(IMessage message)
    {
        await _context.Clients.All.MessageReceived(message);
    }

    public async Task SendMessageToGroupsAsync(IMessage message, params string[] groups)
    {
        if (groups.Any(z => !string.IsNullOrEmpty(z)))
            await _context.Clients.Groups(groups).MessageReceived(message);
    }

    public async Task SendMessageAsync(IMessage message, Action<MessageConfigBuilder>? configuration = null)
    {
        var builder = new MessageConfigBuilder();
        configuration?.Invoke(builder);
        var config = builder.Build();

        if (config.Groups.Count > 0 && !config.ExceptCaller)
            await SendMessageToGroupsAsync(message, config.Groups.ToArray());

        if (config.Groups.Count > 0 && config.ExceptCaller)
            await SendMessageToGroupsExceptCallerAsync(message, config.Caller, config.Groups.ToArray());

        if (config.ToCaller)
            await SendMessageToCallerAsync(message, config.Caller);

        if (config.IsBroadcastMessage)
            await SendMessageToAllAsync(message);

    }

    public async Task SendMessageToGroupsExceptCallerAsync(IMessage message, string caller, params string[] groups)
    {
        if (_hubStore.TryGetUserConnectionIds(caller, out var connectionIds))
        {
            foreach (var group in groups)
            {
                if (groups.Any(z => !string.IsNullOrEmpty(z)))
                    await _context.Clients.GroupExcept(group, connectionIds).MessageReceived(message);
            }
        }
        else
        {
            await SendMessageToGroupsAsync(message, groups);
        }
    }
}

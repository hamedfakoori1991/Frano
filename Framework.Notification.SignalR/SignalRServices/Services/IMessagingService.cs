using Framework.Tools.Messaging.SignalRServices.Contracts;

namespace Framework.Tools.Messaging.SignalRServices.Services;

public interface IMessagingService
{
    Task SendMessageToCallerAsync(IMessage message, string caller);
    Task SendMessageToClientAsync(IMessage message, string connectionId);
    Task SendMessageToAllAsync(IMessage message);
    Task SendMessageToGroupsAsync(IMessage message, params string[] groups);
    Task SendMessageToGroupsExceptCallerAsync(IMessage message, string caller, params string[] groups);
    Task SendMessageAsync(IMessage message, Action<MessageConfigBuilder>? configuration = null);
}

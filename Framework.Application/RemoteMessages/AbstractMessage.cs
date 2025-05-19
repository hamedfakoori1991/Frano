using Framework.Domain.Entities;
using Framework.Tools.Messaging.SignalRServices.Contracts;

namespace Framework.Application.RemoteMessages;

public abstract class AbstractMessage<T> : IMessage where T : class
{
    protected AbstractMessage()
    {
        DateTime = DateTime.Now;
        Id = Guid.NewGuid();
        Type = typeof(T).Name;
    }

    public Guid Id { get; set; }
    public DateTime DateTime { get; }
    public string Type { get; }
    public List<MessageResponse> Messages { get; set; } = new();

}

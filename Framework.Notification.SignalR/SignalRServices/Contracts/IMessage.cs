namespace Framework.Tools.Messaging.SignalRServices.Contracts;

public interface IMessage
{
    Guid Id { get; }
    DateTime DateTime { get; }
    string Type { get; }
}

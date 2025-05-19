using Framework.Tools.Messaging.SignalRServices.Contracts;

public interface IMessageHub
{
    Task MessageReceived(IMessage message); // do not change the name to async
}



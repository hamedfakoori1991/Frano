namespace Framework.Tools.Messaging.SignalRServices.Services;

public class MessageConfig
{
    public bool ToCaller { get; set; } = true;
    public List<string> Groups { get; set; } = new List<string>();
    public bool IsBroadcastMessage { get; set; } = false;
    public string Caller { get; set; }
    public bool ExceptCaller { get; set; } = false;
}

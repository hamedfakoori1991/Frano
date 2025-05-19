namespace Framework.Tools.Messaging.SignalRServices.Services;

public class MessageConfigBuilder
{
    private MessageConfig _config = new();

    public MessageConfigBuilder ToGroups(params string[] groups)
    {
        _config.Groups.AddRange(groups);
        return this;
    }
    public MessageConfigBuilder ToGroupsExceptCaller(string caller, params string[] groups)
    {
        _config.Groups.AddRange(groups);
        _config.ExceptCaller = true;
        _config.Caller = caller;
        return this;
    }
    public MessageConfigBuilder IsBroadcast()
    {
        _config.IsBroadcastMessage = true;
        return this;
    }
    public MessageConfigBuilder ToCaller(string caller)
    {
        _config.ToCaller = true;
        _config.Caller = caller;
        return this;
    }
    public MessageConfig Build()
    {
        return _config;
    }
}

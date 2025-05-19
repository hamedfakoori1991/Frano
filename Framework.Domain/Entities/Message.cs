namespace Framework.Domain.Entities;

public class Message
{
    public string Code { get; init; }
    private string _text;
    public string Text
    {
        get
        {
            if (Params != null) // TODO: placeholders and params matching
                return string.Format(_text, Params);

            return _text;
        }

        set => _text = value;
    }

    public string Description { get; init; }
    public MessageType Type { get; set; }
    public ErrorType? ErrorType { get; set; }
    public object[] Params { get; private set; } = null;

    public Message()
    {

    }
    public static Message operator +(Message a, Message b)
    {
        return CreateError(a.Code, a.Text + "\n" + b.Text, a.Description + "\n" + b.Description,
            a.ErrorType ?? default(ErrorType));
    }

    private Message(string code, string text, string description, MessageType type, ErrorType? errorType = default)
    {
        Code = code;
        Text = text;
        Description = description;
        Type = type;
        ErrorType = errorType;
    }
    public static Message CreateError(string code, string text, string description, ErrorType errorType)
    {
        return new Message(code, text, description, MessageType.Error, errorType);
    }
    public static Message CreateSuccess(string code, string text, string description)
    {
        return new Message(code, text, description, MessageType.Success);
    }
    public static Message CreateWarning(string code, string text, string description)
    {
        return new Message(code, text, description, MessageType.Warning);
    }
    public Message WithParams(params object[] parameters)
    {
        Params = parameters;
        return this;
    }
    public override string ToString()
    {
        if (Params != null) // TODO: placeholders and params matching
            return string.Format(_text, Params);

        return _text;
    }

}

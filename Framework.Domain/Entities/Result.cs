using Framework.Domain.Interfaces;

namespace Framework.Domain.Entities;

public class Result<T> : Result, IResult<T>
{
    public T Data { get; set; } = default!;
    public Result()
    {
    }
    public Result(T data, List<Message> messages)
    {
        Data = data;
        Messages = messages;
    }

    public static Result<T> Ok(T data, params Message[] messages)
    {
        return new Result<T>(data, messages.ToList());
    }
    public static Result<T> Ok(T data)
    {
        return new Result<T>(data, new());

    }
    public static Result<T> Failed(params Message[] messages)
    {
        var res = new Result<T>();
        res.Messages.AddRange(messages.ToList());
        res.Status = MessageType.Error;
        return res;
    }
    public static Result<T> Failed(List<Message> messages)
    {
        var res = new Result<T>();
        res.Messages.AddRange(messages);
        res.Status = MessageType.Error;
        return res;
    }

    public static implicit operator Result<T>(T value)
    {
        return Result<T>.Ok(value);
    }
    public static implicit operator Result<T>(Message value)
    {
        return Result<T>.Failed(value);
    }
    public static implicit operator Result<T>(List<Message> values)
    {
        return Result<T>.Failed(values.ToArray());
    }
    public static implicit operator Result<T>((T value, List<Message> messages) tuple)
    {
        return new Result<T>(tuple.value, tuple.messages);
    }
}

public class Result
{
    public MessageType Status { get; set; }
    public ErrorType? ErrorType { get; set; } = null;
    public List<Message> Messages { get; set; } = new();
    public bool HasError => Messages != null && Messages.Any() && Status == MessageType.Error;
    public bool IsSuccess => Status == MessageType.Success;

    protected Result(List<Message> messages, MessageType status)
    {
        Messages = messages;
        Status = status;
    }
    public Result()
    {
    }

    public static Result Ok(List<Message> messages)
    {
        return new Result(messages.ToList(), MessageType.Success);
    }
    public static Result Ok(params Message[] messages)
    {
        return new Result(messages.ToList(), MessageType.Success);
    }

    public static Result Failed(List<Message> messages)
    {
        return new Result(messages, MessageType.Error);
    }
    public static Result Failed(params Message[] messages)
    {
        return new Result(messages.ToList(), MessageType.Error);
    }
    public Result AddMessage(params Message[] messages)
    {
        Messages.AddRange(messages);
        return this;
    }
    public Result WithErrorType(ErrorType errorType)
    {
        ErrorType = errorType;
        return this;
    }
    public static implicit operator Result(List<Message> messages)
    {
        return Failed(messages);
    }
    public static implicit operator Result(Message message)
    {
        return Failed(new List<Message> { message });
    }

}

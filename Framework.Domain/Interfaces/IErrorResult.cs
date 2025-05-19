namespace Framework.Domain.Interfaces;

public interface IErrorResult
{
    Message Error { get; }
    bool HasError { get; }
}

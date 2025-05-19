using Framework.Domain.Entities;
using Framework.Domain.Interfaces;
using Mapster;

namespace Framework.WebApi.Contracts;

public class ApiResult<T> : IDataResult<T>
{
    public T Data { get; set; }
    public List<MessageResponse> Messages { get; private set; } = new();

    public static implicit operator ApiResult<T>(Result<T> value)
    {
        return new ApiResult<T>()
        {
            Data = value.Data,
            Messages =  value.Messages.Adapt<List<MessageResponse>>(),
        };
    }

}

using Framework.Domain.Entities;

namespace Framework.WebApi.Contracts;

public record ApiMessageResponse
{
    public int StatusCode { get; set; }
    public List<MessageResponse> Messages { get; set; } = new();
}


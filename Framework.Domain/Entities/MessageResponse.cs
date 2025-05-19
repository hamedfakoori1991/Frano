
namespace Framework.Domain.Entities;

public record struct MessageResponse
{
    public MessageResponse()
    {
    }

    public string Type { get; init; } = null ?? string.Empty;
    public string Title { get; init; } = null ?? string.Empty;
    public string Detail { get; init; } = null ?? string.Empty;
    public MessageType MessageType { get; set; }

    public static implicit operator MessageResponse(Message value)
    {
        return new MessageResponse
        {
            Type = value.Code,
            Title = value.Text,
            Detail = value.Description,
            MessageType = value.Type
        };
    }
}


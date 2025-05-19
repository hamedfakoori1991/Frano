using System.Reflection;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace Framework.Messaging.MassTransit.MassTransitServices.Services;

public class MessageSerializer<T> : IAsyncSerializer<T>
{
    private readonly Assembly? assembly;

    public MessageSerializer(Assembly? assembly)
    {
        this.assembly = assembly;
    }
    public Task<byte[]> SerializeAsync(T data, SerializationContext context)
    {
        string json;
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            TypeInfoResolver = new MessageTypeInfoResolver<T>(assembly ?? typeof(T).Assembly)
        };
        json = JsonSerializer.Serialize(data, typeof(T), options);
        return Task.FromResult(Encoding.UTF8.GetBytes(json));
    }
}

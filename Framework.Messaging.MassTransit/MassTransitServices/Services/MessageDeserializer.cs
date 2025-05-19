using System.Reflection;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Framework.Messaging.MassTransit.MassTransitServices.Services;

public class MessageDeserializer<T> : IDeserializer<T>
{
    private readonly JsonSerializerOptions _options;

    public MessageDeserializer(Assembly assembly)
    {
        _options = new JsonSerializerOptions
        {
            TypeInfoResolver = new MessageTypeInfoResolver<T>(assembly)
        };
    }

    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (isNull || data.IsEmpty)
        {
            throw new ArgumentNullException(nameof(data), "Data cannot be null or empty.");
        }

        var json = Encoding.UTF8.GetString(data);

        var deserialized = JsonSerializer.Deserialize<T>(json, _options);

        if (deserialized == null)
        {
            throw new InvalidOperationException($"Failed to deserialize the JSON to {nameof(T)}.");
        }

        return deserialized;
    }
}

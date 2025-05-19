using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Framework.Messaging.MassTransit.MassTransitServices.Services;

public class MessageTypeInfoResolver<T> : DefaultJsonTypeInfoResolver
{
    private Assembly? _assembly;

    public MessageTypeInfoResolver(Assembly? assembly = default)
    {
        _assembly = assembly;
    }
    public override JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        if (type == typeof(T))
        {
            var jsonTypeInfo = base.GetTypeInfo(type, options);
            if (jsonTypeInfo is not null)
            {
                jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                {
                    TypeDiscriminatorPropertyName = "TypeDiscriminatorProperty",
                    IgnoreUnrecognizedTypeDiscriminators = false,
                };

                foreach (var derivedType in GetDerivedTypes<T>())
                {
                    jsonTypeInfo.PolymorphismOptions.DerivedTypes.Add(derivedType);
                }

                return jsonTypeInfo;
            }
        }
        return base.GetTypeInfo(type, options);
    }


    private IList<JsonDerivedType> GetDerivedTypes<TBase>()
    {
        var derivedTypes = new List<JsonDerivedType>();

        var baseType = typeof(TBase);
        if (_assembly == null)
            _assembly = baseType.Assembly;

        foreach (var type in _assembly.GetTypes())
        {
            if (type.IsAssignableTo(baseType) && !type.IsAbstract)
            {
                derivedTypes.Add(new JsonDerivedType(type, type.Name));
            }
        }

        return derivedTypes;
    }
}

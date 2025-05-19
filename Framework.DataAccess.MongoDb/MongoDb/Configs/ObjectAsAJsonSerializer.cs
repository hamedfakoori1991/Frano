using MongoDB.Bson.Serialization;

namespace Framework.DataAccess.Databases.MongoDb.Configs;

internal class ObjectAsAJsonSerializer : IBsonSerializer
{
    public Type ValueType => typeof(object);

    public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var json = context.Reader.ReadString();
        return MongoJsonSerializer.Deserialize(json);
    }

    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        var json = MongoJsonSerializer.Serialize(value);
        context.Writer.WriteString(json.ToString());
    }
}

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Framework.DataAccess.Databases.MongoDb.Configs;
public class GuidSerializationProvider : IBsonSerializationProvider
{
    public IBsonSerializer GetSerializer(Type type)
    {
        if (type == typeof(Guid))
        {
            return new GuidSerializer(BsonType.String);
        }

        return null;
    }
}

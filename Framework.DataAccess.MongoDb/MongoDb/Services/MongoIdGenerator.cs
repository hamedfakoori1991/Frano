using Framework.Application.Interfaces;
using MongoDB.Bson;

namespace Framework.DataAccess.MongoDb.MongoDb.Services;

public class MongoIdGenerator : IdGenerator
{
    public string NewId()
    {
        return  ObjectId.GenerateNewId().ToString();
    }
}

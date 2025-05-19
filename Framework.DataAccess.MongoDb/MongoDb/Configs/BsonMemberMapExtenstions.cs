using System.Collections;
using MongoDB.Bson.Serialization;

namespace Framework.DataAccess.Databases.MongoDb.Configs;

public static class BsonMemberMapExtenstions
{
    public static void IgnoreNavigationList(this BsonMemberMap bsonMemberMap)
    {
        bsonMemberMap.SetShouldSerializeMethod(member =>
         {
             if (member is IEnumerable collection)
             {
                 return collection.GetEnumerator().MoveNext();
             }

             return false;
         });
    }

    public static void IgnoreNavigationProperty(this BsonMemberMap bsonMemberMap)
    {
        bsonMemberMap.SetShouldSerializeMethod(member =>
        {
            return false;
        });
    }


}


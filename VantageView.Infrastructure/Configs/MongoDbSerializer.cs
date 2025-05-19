using Framework.DataAccess.Databases.MongoDb.Configs;
using Framework.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using VantageView.Domain.CrawlerConfigurations;


namespace VantageView.Infrastructure.Configs;
public partial class MongoDbSerializer : IMongoDbSerializer
{
    public void Register()
    {

        if (!BsonClassMap.IsClassMapRegistered(typeof(CrawlerConfiguration)))
            BsonClassMap.RegisterClassMap<CrawlerConfiguration>(cm =>
            {
                    BsonClassMap.RegisterClassMap<BaseEntity<string>>(cm =>
                    {
                        cm.AutoMap();
                        cm.MapIdMember(z => z.Id).SetSerializer(new StringSerializer(BsonType.ObjectId));
                    });

                cm.AutoMap();
            });

    }
}


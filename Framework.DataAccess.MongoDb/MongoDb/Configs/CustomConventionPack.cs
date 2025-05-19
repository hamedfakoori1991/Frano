using MongoDB.Bson.Serialization.Conventions;

namespace Framework.DataAccess.Databases.MongoDb.Configs;

public class CustomConventionPack : IConventionPack
{
    private CustomConventionPack()
    {
        Conventions =
            [
                new ReadWriteMemberFinderConvention(),
                new NamedIdMemberConvention(new [] { "_id" }),
                new NamedExtraElementsMemberConvention(new [] { "ExtraElements" }),
                new IgnoreExtraElementsConvention(false),
                new ImmutableTypeClassMapConvention(),
                new NamedParameterCreatorMapConvention(),
                new StringObjectIdIdGeneratorConvention(),
                new LookupIdGeneratorConvention(),
                new CamelCaseElementNameConvention()
            ];
    }

    public static void OverrideDefaultConventionPack()
    {
        ConventionRegistry.Remove("__defaults__");
        ConventionRegistry.Register(nameof(CustomConventionPack), new CustomConventionPack(), t => true);
    }

    public IEnumerable<IConvention> Conventions { get; }
}


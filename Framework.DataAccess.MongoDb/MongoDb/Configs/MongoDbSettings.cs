namespace Framework.DataAccess.Databases.MongoDb.Configs;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string Database { get; set; } = null!;
    public string DatabaseNameFormat { get; set; } = null!;
}

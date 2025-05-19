using System.Text.Json;

namespace Framework.DataAccess.Databases.MongoDb.Configs;

public static class MongoJsonSerializer
{
    public static object Deserialize(object config)
    {
        if (config == null || config is not string jsonString || string.IsNullOrWhiteSpace(jsonString))
            return string.Empty;

        return JsonSerializer.Deserialize<object>(config.ToString());
    }
    public static object Serialize(object config)
    {
        return JsonSerializer.Serialize(config);
    }
}

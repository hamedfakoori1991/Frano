namespace Framework.Messaging.MassTransit.Settings;


public class KafkaSettings
{
    public const string Name = "KafkaSettings";
    public string ConnectionString { get; set; } = null!;
    public string GroupId { get; set; } = null!;
    public List<KafkaTopicConfiguration> Topics { get; set; }

}

public class KafkaTopicConfiguration
{
    public string Name { get; set; } = null!;
    public int Partitions { get; set; } = 1; // Default to 1 partition
    public short ReplicationFactor { get; set; } = 1; // Default to a replication factor of 1
    public Dictionary<string, string> Configs { get; set; } = new Dictionary<string, string>();
}

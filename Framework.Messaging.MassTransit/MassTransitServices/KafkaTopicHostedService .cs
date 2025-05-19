using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Framework.Messaging.MassTransit.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Framework.Messaging.MassTransit.MassTransitServices;
public class KafkaTopicHostedService : IHostedService
{
    private readonly ILogger<KafkaTopicHostedService> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public KafkaTopicHostedService(ILogger<KafkaTopicHostedService> logger, KafkaSettings kafkaSettings)
    {
        _logger = logger;
        _kafkaSettings = kafkaSettings;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = _kafkaSettings.ConnectionString }).Build();
        var topics = new List<string> ();

        foreach (var topicName in topics)
        {
            try
            {
                var metadata = adminClient.GetMetadata(topicName, TimeSpan.FromSeconds(5));
                if (metadata.Topics.Find(t => t.Topic == topicName) == null)
                {
                    _logger.KafkaTopicNotFound(topicName);
                    var topicSpecification = new TopicSpecification
                    {
                        Name = topicName,
                        NumPartitions = 1,
                        ReplicationFactor = 1
                    };
                    await adminClient.CreateTopicsAsync(new List<TopicSpecification> { topicSpecification });
                    _logger.KafkaTopicCreated(topicName);
                }
                else
                {
                    _logger.KafkaTopicExists(topicName);
                }
            }
            catch (CreateTopicsException e)
            {
                _logger.KafkaTopicCreationError(topicName, e.Results[0].Error.Reason);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}




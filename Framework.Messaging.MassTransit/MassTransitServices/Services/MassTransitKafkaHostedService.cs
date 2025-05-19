using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Framework.Infrastructure.Messaging.MassTransitServices;
using Framework.Messaging.MassTransit.Settings;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Framework.Messaging.MassTransit.MassTransitServices.Services;

public class MassTransitKafkaHostedService : IHostedService
{
    private readonly ILogger<MassTransitKafkaHostedService> _logger;
    private readonly IOptions<KafkaSettings> _kafkaOptions;
    private readonly IBusControl _busControl;
    private readonly IConfiguration _configuration;

    public MassTransitKafkaHostedService(ILogger<MassTransitKafkaHostedService> logger, IOptions<KafkaSettings> kafkaOptions, IBusControl busControl, IConfiguration configuration)
    {
        _logger = logger;
        _kafkaOptions = kafkaOptions;
        _busControl = busControl;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var mteName = GetMteName(_configuration);
            var options = _kafkaOptions.Value;
            using var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = options.ConnectionString }).Build();

            if (options.Topics.Count == 0)
                _logger.LogWarning("The Topic list is empty");

            var existingTopics = adminClient.GetMetadata(TimeSpan.FromSeconds(10));

            var topics = new List<string> { TopicConsts.ExecutionPlanTopicName, TopicConsts.DownloadTopicName };

            foreach (var topic in topics)
            {
                bool topicExists = existingTopics.Topics.Any(t => t.Topic == $"{topic}.{mteName}");

                if (!topicExists)
                {
                    var aSpecification = new TopicSpecification
                    {
                        Name = $"{topic}.{mteName}",
                    };

                    await adminClient.CreateTopicsAsync(new List<TopicSpecification> { aSpecification });
                }
            }

            foreach (var topicConfig in options.Topics)
            {
                var metadata = adminClient.GetMetadata(topicConfig.Name, TimeSpan.FromSeconds(5));
                if (metadata.Topics.Find(t => t.Topic == topicConfig.Name && t.Partitions.Count > 0) == null)
                {
                    var topicSpecification = new TopicSpecification
                    {
                        Name = $"{topicConfig.Name}.{mteName}",
                        NumPartitions = topicConfig.Partitions,
                        ReplicationFactor = topicConfig.ReplicationFactor,
                        Configs = topicConfig.Configs,
                    };

                    await adminClient.CreateTopicsAsync(new List<TopicSpecification> { topicSpecification });
                }
            }

            await _busControl.StartAsync(cancellationToken);

        }
        catch (CreateTopicsException ex)
        {
            _logger.LogError("Failed to create the topic. Aborting MassTransit setup.");
            _logger.LogError(ex.Message, ex.InnerException);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message, ex.InnerException);
        }

    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Stopping MassTransit bus...");
            await _busControl.StopAsync(cancellationToken);
            _logger.LogInformation("MassTransit bus stopped successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error stopping MassTransit bus: {ex.Message}");
        }

    }

    public static string GetMteName(IConfiguration configuration) => configuration != null ? configuration["DatabaseMte"] ?? string.Empty : string.Empty;

}

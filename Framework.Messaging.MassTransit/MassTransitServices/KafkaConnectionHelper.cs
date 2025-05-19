using Confluent.Kafka;
using Framework.Messaging.MassTransit.Settings;
using Microsoft.Extensions.Logging;

namespace Framework.Messaging.MassTransit.MassTransitServices;

public static class KafkaConnectionHelper
{
    /// <summary>
    /// Tries to connect to Kafka to verify its availability.
    /// </summary>
    /// <param name="kafkaSetting">Kafka settings including connection string.</param>
    /// <param name="logger">Optional logger for reporting issues.</param>
    /// <returns>True if Kafka is available; otherwise, false.</returns>
    public static bool TryConnectKafka(KafkaSettings? kafkaSetting, ILogger? logger = null)
    {
        if (kafkaSetting == null || string.IsNullOrEmpty(kafkaSetting.ConnectionString))
        {
            logger.LogError($"kafka setting is null");
            return false;
        }

        try
        {
            using var adminClient = new AdminClientBuilder(new AdminClientConfig
            {
                BootstrapServers = kafkaSetting.ConnectionString
            }).Build();

            adminClient.GetMetadata(TimeSpan.FromSeconds(5));

            return true;
        }
        catch (KafkaException ex)
        {
            logger.LogError("Failed to connect to Kafka. Aborting MassTransit setup.");
            logger.LogError(ex, ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to connect to Kafka. Aborting MassTransit setup.");
            logger.LogError(ex, ex.Message);
            return false;
        }
    }
}

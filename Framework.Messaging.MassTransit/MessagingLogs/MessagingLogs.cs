using Microsoft.Extensions.Logging;

public static partial class MessagingLogs
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Warning, Message = "The Topic {TopicName} is not found.", SkipEnabledCheck = false)]
    public static partial void KafkaTopicNotFound(this ILogger logger, string topicName);

    [LoggerMessage(EventId = 2, Level = LogLevel.Information, Message = "Topic {TopicName} already exists.", SkipEnabledCheck = false)]
    public static partial void KafkaTopicExists(this ILogger logger, string topicName);

    [LoggerMessage(EventId = 3, Level = LogLevel.Information, Message = "Topic {TopicName} created successfully", SkipEnabledCheck = false)]
    public static partial void KafkaTopicCreated(this ILogger logger, string topicName);


    [LoggerMessage(EventId = 4, Level = LogLevel.Warning, Message = "Error occured while creating Topic {TopicName} reason : {Reason}", SkipEnabledCheck = false)]
    public static partial void KafkaTopicCreationError(this ILogger logger, string topicName, string reason);
}

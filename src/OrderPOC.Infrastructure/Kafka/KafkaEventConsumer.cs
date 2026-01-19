using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrderPOC.Application.Kafka;
using Polly;
using Polly.Retry;

namespace OrderPOC.Infrastructure.Kafka;

public class KafkaEventConsumer<T> : IEventConsumer<T>
{
    private static readonly int MaxRetryAttempts = 3;
    private readonly ConsumerConfig _config;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ILogger<KafkaEventConsumer<T>> _logger;
    private readonly IEventProducer _producer;

    public KafkaEventConsumer(
        IConfiguration configuration,
        ILogger<KafkaEventConsumer<T>> logger,
        IEventProducer producer,
        string groupId)
    {
        _logger = logger;
        _producer = producer;
        _config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        // Define Policy: Retry 3 times with exponential backoff (2s, 4s, 8s)
        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(MaxRetryAttempts, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) => 
                {
                    // Log retries! valuable for debugging
                    _logger.LogWarning("Retry {RetryCount} encountered error: {Message}. Waiting {timeSpan}...", retryCount, exception.Message, timeSpan);
                });
        
    }

    public Task ConsumeAsync(
        string topic,
        Func<T, Task> handler,
        CancellationToken token)
    {
        // Vital: Wrap the blocking loop in Task.Run to avoid blocking the main thread
        return Task.Run(async () =>
        {
            using var consumer = new ConsumerBuilder<string, string>(_config).Build();
            consumer.Subscribe(topic);

            try
            {
                while (!token.IsCancellationRequested)
                {
                    ConsumeResult<string, string>? result = null;
                    try
                    {
                        // Consume is blocking, but it's okay inside Task.Run
                        result = consumer.Consume(token); 
                        
                        if (result?.Message?.Value != null)
                        {
                            var message = JsonSerializer.Deserialize<T>(result.Message.Value);
                            if (message != null)
                            {
                                // Execute handler INSIDE the policy
                                await _retryPolicy.ExecuteAsync(async () => 
                                {
                                    await handler(message);
                                });

                                // Only commit if successful (or retries exhausted and handled)
                                consumer.Commit(result);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Message failed after retries. Moving to DLT.");

                        if (result != null)
                        {
                            var dltTopic = $"{topic}.dlt";
                            
                            // 1. Publish to DLT (Wrap in try-catch to avoid crashing loop if DLT fails)
                            try
                            {
                                // Pass the raw JSON string to preserve exactly what failed
                                // Using generic T here might need your Producer to support raw string or deserialized T
                                // Assuming Producer accepts T:
                                if (result.Message.Value != null)
                                {
                                   var failedMessage = JsonSerializer.Deserialize<T>(result.Message.Value);
                                   if(failedMessage != null)
                                        await _producer.PublishAsync(dltTopic, failedMessage);
                                }
                                
                                _logger.LogInformation("Moved message to {DltTopic}", dltTopic);
                            }
                            catch(Exception dltEx)
                            {
                                _logger.LogCritical(dltEx, "CRITICAL: Failed to publish to DLT!");
                            }

                            // 2. Commit the offset so we don't process this "poison" message again
                            consumer.Commit(result);
                        }
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }, token);
    }
}

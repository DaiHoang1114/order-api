using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace OrderPOC.Infrastructure.Kafka;

public class KafkaEventConsumer<T> : IEventConsumer<T>
{
    private readonly ConsumerConfig _config;

    public KafkaEventConsumer(IConfiguration configuration, string groupId)
    {
        _config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
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
                    try
                    {
                        // Consume is blocking, but it's okay inside Task.Run
                        var result = consumer.Consume(token); 
                        
                        if (result?.Message?.Value != null)
                        {
                            var message = JsonSerializer.Deserialize<T>(result.Message.Value);
                            if (message != null)
                            {
                                await handler(message);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception)
                    {
                        // Log (or ignore) serialization errors to keep consumer alive
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

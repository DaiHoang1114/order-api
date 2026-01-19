using Confluent.Kafka;
using System.Text.Json;
using OrderPOC.Application.Kafka;
using Microsoft.Extensions.Configuration;

namespace OrderPOC.Infrastructure.Kafka;

public class KafkaProducer : IEventProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration configuration)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, T message)
    {
        var json = JsonSerializer.Serialize(message);

        await _producer.ProduceAsync(
            topic,
            new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = json
            });
    }

}
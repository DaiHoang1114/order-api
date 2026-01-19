namespace OrderPOC.Application.Kafka;

public interface IEventProducer
{
    Task PublishAsync<T>(string topic, T message);
}
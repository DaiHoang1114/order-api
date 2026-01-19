namespace OrderPOC.Infrastructure.Kafka;

public interface IEventConsumer<T>
{
    Task ConsumeAsync(
        string topic,
        Func<T, Task> handler,
        CancellationToken token);
}

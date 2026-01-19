using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderPOC.Application.Orders.Events;
using OrderPOC.Infrastructure.Kafka;

namespace OrderPOC.Infrastructure.BackgroundJobs;

public class OrderCreatedInventoryConsumer(
    [FromKeyedServices("Inventory")] IEventConsumer<OrderCreatedIntegrationEvent> consumer,
    ILogger<OrderCreatedInventoryConsumer> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return consumer.ConsumeAsync(
            topic: "order.created",
            handler: UpdateInventoryAsync,
            token: stoppingToken);
    }

    private Task UpdateInventoryAsync(OrderCreatedIntegrationEvent evt)
    {
        logger.LogInformation("Updating inventory for Order {OrderId}", evt.OrderId);
        return Task.CompletedTask;
    }
}

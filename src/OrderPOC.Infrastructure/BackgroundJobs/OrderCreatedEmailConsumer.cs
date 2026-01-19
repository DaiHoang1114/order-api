using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderPOC.Application.Orders.Events;
using OrderPOC.Infrastructure.Kafka;

namespace OrderPOC.Infrastructure.BackgroundJobs;

public class OrderCreatedEmailConsumer(
    [FromKeyedServices("Email")] IEventConsumer<OrderCreatedIntegrationEvent> consumer,
    ILogger<OrderCreatedEmailConsumer> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return consumer.ConsumeAsync(
            topic: "order.created",
            handler: SendEmailAsync,
            token: stoppingToken);
    }

    private Task SendEmailAsync(OrderCreatedIntegrationEvent evt)
    {
        logger.LogInformation("Sending email for Order {OrderId} to Customer {CustomerId}", evt.OrderId, evt.CustomerId);
        return Task.CompletedTask;
    }
}
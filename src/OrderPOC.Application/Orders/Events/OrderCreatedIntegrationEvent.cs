namespace OrderPOC.Application.Orders.Events;

public record OrderCreatedIntegrationEvent(
    Guid OrderId,
    Guid CustomerId,
    DateTime CreatedAt
);
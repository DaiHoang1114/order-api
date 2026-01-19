namespace OrderPOC.Domain.Orders.Events;

public record OrderCreatedDomainEvent(
    Guid OrderId,
    Guid CustomerId,
    DateTime CreatedAtUtc
);
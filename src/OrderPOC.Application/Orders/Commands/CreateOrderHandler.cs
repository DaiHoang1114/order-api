using MediatR;
using OrderPOC.Domain.Orders;
using OrderPOC.Application.Repositories;
using OrderPOC.Application.Orders.Events;
using OrderPOC.Application.Kafka;

namespace OrderPOC.Application.Orders.Commands;

public sealed class CreateOrderHandler(
    IOrderRepository orderRepository,
    IEventProducer producer)
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order(request.CustomerId);
        await orderRepository.AddAsync(order);

        await orderRepository.SaveChangesAsync(cancellationToken);

        // Publish integration event
        var evt = new OrderCreatedIntegrationEvent(
            order.Id,
            order.CustomerId,
            DateTime.UtcNow);

        _ = producer.PublishAsync(
            "order.created",
            evt);

        return order.Id;
    }
}
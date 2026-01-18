using MediatR;
using OrderPOC.Domain.Orders;
using OrderPOC.Application.Repositories;

namespace OrderPOC.Application.Orders.Commands;

public sealed class CreateOrderHandler(
    IOrderRepository orderRepository)
    : IRequestHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order(request.CustomerId);
        await orderRepository.AddAsync(order);

        return order.Id;
    }
}
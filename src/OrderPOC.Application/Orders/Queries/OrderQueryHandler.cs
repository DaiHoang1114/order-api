using MediatR;
using OrderPOC.Application.Queryable;
using OrderPOC.Application.Exceptions;

namespace OrderPOC.Application.Orders.Queries;

public sealed class OrderQueryHandler(
    IOrderQueryable orderQueryable)
    : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await orderQueryable.GetByIdAsync(request.OrderId, cancellationToken)
            ?? throw new NotFoundException("Order with id not found");

        return new OrderDto(
            order.Id,
            order.CustomerId,
            order.CreatedAt);
    }
}
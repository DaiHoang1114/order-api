using MediatR;

namespace OrderPOC.Application.Orders.Queries;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;
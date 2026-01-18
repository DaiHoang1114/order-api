using MediatR;

namespace OrderPOC.Application.Orders.Commands;

public record CreateOrderCommand(Guid CustomerId) : IRequest<Guid>;
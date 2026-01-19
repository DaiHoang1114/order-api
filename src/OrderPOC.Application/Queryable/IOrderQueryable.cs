using OrderPOC.Domain.Orders;

namespace OrderPOC.Application.Queryable;

public interface IOrderQueryable
{
    Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct);
}
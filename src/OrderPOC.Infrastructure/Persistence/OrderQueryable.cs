using OrderPOC.Domain.Orders;
using OrderPOC.Application.Queryable;
using Microsoft.EntityFrameworkCore;

namespace OrderPOC.Infrastructure.Persistence;

public class OrderQueryable(OrderDbContext db) : IOrderQueryable
{
    public async Task<Order?> GetByIdAsync(Guid orderId, CancellationToken ct)
    {
        return await db.Orders.SingleOrDefaultAsync(o => o.Id == orderId, ct);
    }
}
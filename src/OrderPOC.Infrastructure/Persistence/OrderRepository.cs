using OrderPOC.Domain.Orders;
using OrderPOC.Application.Repositories;

namespace OrderPOC.Infrastructure.Persistence;
 
public class OrderRepository(OrderDbContext db) : IOrderRepository
{
    public void Add(Order order)
    {
        db.Orders.Add(order);
    }

    public async Task AddAsync(Order order)
    {
        await db.Orders.AddAsync(order);
    }

    public Task SaveChangesAsync(CancellationToken ct)
    {
        return db.SaveChangesAsync(ct);
    }
}
 
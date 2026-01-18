 using OrderPOC.Domain.Orders;
 using System.Threading;
 using System.Threading.Tasks;

namespace OrderPOC.Application.Repositories;

public interface IOrderRepository
{
    void Add(Order order);
    Task AddAsync(Order order);
    Task SaveChangesAsync(CancellationToken ct);
}
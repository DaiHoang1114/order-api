using Microsoft.EntityFrameworkCore;
using OrderPOC.Domain.Orders;

namespace OrderPOC.Infrastructure.Persistence;


public class OrderDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();

    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options) { }
}
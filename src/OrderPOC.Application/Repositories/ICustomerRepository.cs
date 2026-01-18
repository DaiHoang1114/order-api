using OrderPOC.Domain.Customers;

namespace OrderPOC.Application.Repositories;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid customerId);
}
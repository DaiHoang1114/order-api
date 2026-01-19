namespace OrderPOC.Domain.Customers;

public class Customer
{
    public Guid Id { get; private set; }
    public required string Name { get; set; }

    private Customer() { } // EF Core

    public Customer(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}
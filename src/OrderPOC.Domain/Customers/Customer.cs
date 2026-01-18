namespace OrderPOC.Domain.Customers;

public class Customer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private Customer() { } // EF Core

    public Customer(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}
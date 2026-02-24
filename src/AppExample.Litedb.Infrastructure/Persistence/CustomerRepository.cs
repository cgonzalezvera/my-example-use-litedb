using AppExample.Litedb.Domain.Entities;
using AppExample.Litedb.Domain.Repositories;
using AppExample.Litedb.Infrastructure.Persistence;

namespace AppExample.Litedb.Infrastructure.Persistence;

public class CustomerRepository(LiteDbContext context) : ICustomerRepository
{
    private const string CollectionName = "customers";

    public Customer? GetById(string id) =>
        context.GetCollection<Customer>(CollectionName).FindById(id);

    public IEnumerable<Customer> GetAll() =>
        context.GetCollection<Customer>(CollectionName).FindAll();

    public void Add(Customer customer) =>
        context.GetCollection<Customer>(CollectionName).Insert(customer);

    public void Update(Customer customer) =>
        context.GetCollection<Customer>(CollectionName).Update(customer);
}

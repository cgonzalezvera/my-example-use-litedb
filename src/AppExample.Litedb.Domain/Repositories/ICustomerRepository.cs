using AppExample.Litedb.Domain.Entities;

namespace AppExample.Litedb.Domain.Repositories;

public interface ICustomerRepository
{
    Customer? GetById(string id);
    IEnumerable<Customer> GetAll();
    void Add(Customer customer);
    void Update(Customer customer);
}

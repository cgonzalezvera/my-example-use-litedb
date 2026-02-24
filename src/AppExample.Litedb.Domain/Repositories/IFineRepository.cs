using AppExample.Litedb.Domain.Entities;

namespace AppExample.Litedb.Domain.Repositories;

public interface IFineRepository
{
    void Add(Fine fine);
    IEnumerable<Fine> GetByCustomer(string customerId);
}

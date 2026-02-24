using AppExample.Litedb.Domain.Entities;
using AppExample.Litedb.Domain.Repositories;
using AppExample.Litedb.Infrastructure.Persistence;

namespace AppExample.Litedb.Infrastructure.Persistence;

public class FineRepository(LiteDbContext context) : IFineRepository
{
    private const string CollectionName = "fines";

    public void Add(Fine fine) =>
        context.GetCollection<Fine>(CollectionName).Insert(fine);

    public IEnumerable<Fine> GetByCustomer(string customerId) =>
        context.GetCollection<Fine>(CollectionName)
            .Find(f => f.CustomerId == customerId);
}

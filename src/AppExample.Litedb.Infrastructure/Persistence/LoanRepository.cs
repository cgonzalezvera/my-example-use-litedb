using AppExample.Litedb.Domain.Entities;
using AppExample.Litedb.Domain.Enums;
using AppExample.Litedb.Domain.Repositories;
using AppExample.Litedb.Infrastructure.Persistence;

namespace AppExample.Litedb.Infrastructure.Persistence;

public class LoanRepository(LiteDbContext context) : ILoanRepository
{
    private const string CollectionName = "loans";

    public Loan? GetById(string id) =>
        context.GetCollection<Loan>(CollectionName).FindById(id);

    public IEnumerable<Loan> GetAll() =>
        context.GetCollection<Loan>(CollectionName).FindAll();

    public IEnumerable<Loan> GetActive() =>
        context.GetCollection<Loan>(CollectionName)
            .Find(l => l.Status == LoanStatus.Active);

    public void Add(Loan loan) =>
        context.GetCollection<Loan>(CollectionName).Insert(loan);

    public void Update(Loan loan) =>
        context.GetCollection<Loan>(CollectionName).Update(loan);
}

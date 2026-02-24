using AppExample.Litedb.Domain.Entities;

namespace AppExample.Litedb.Domain.Repositories;

public interface ILoanRepository
{
    Loan? GetById(string id);
    IEnumerable<Loan> GetAll();
    IEnumerable<Loan> GetActive();
    void Add(Loan loan);
    void Update(Loan loan);
}

using AppExample.Litedb.Application.Abstractions;
using AppExample.Litedb.Domain.Repositories;

namespace AppExample.Litedb.Application.Queries.GetLoans;

public class GetLoansQueryHandler(
    ILoanRepository loanRepository,
    IBookRepository bookRepository,
    ICustomerRepository customerRepository)
    : IQueryHandler<GetLoansQuery, IEnumerable<LoanDto>>
{
    public IEnumerable<LoanDto> Handle(GetLoansQuery query)
    {
        var loans = query.ActiveOnly
            ? loanRepository.GetActive()
            : loanRepository.GetAll();

        return loans.Select(l =>
        {
            var book = bookRepository.GetById(l.BookId);
            var customer = customerRepository.GetById(l.CustomerId);
            return new LoanDto(
                l.Id,
                l.BookId,
                book?.Title ?? "(desconocido)",
                l.CustomerId,
                customer?.Name ?? "(desconocido)",
                l.StartDate,
                l.EndDate,
                l.ReturnDate,
                l.Status
            );
        });
    }
}

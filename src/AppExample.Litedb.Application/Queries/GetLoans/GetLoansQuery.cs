using AppExample.Litedb.Application.Abstractions;

namespace AppExample.Litedb.Application.Queries.GetLoans;

public record GetLoansQuery(bool ActiveOnly = false) : IQuery<IEnumerable<LoanDto>>;

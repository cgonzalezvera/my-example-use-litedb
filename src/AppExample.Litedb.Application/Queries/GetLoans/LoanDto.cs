using AppExample.Litedb.Domain.Enums;

namespace AppExample.Litedb.Application.Queries.GetLoans;

public record LoanDto(
    string Id,
    string BookId,
    string BookTitle,
    string CustomerId,
    string CustomerName,
    DateTime StartDate,
    DateTime EndDate,
    DateTime? ReturnDate,
    LoanStatus Status
);

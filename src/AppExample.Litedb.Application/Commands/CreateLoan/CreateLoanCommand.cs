using AppExample.Litedb.Application.Abstractions;

namespace AppExample.Litedb.Application.Commands.CreateLoan;

public record CreateLoanCommand(string BookId, string CustomerId, int Days) : ICommand;

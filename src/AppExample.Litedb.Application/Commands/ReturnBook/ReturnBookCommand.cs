using AppExample.Litedb.Application.Abstractions;

namespace AppExample.Litedb.Application.Commands.ReturnBook;

public record ReturnBookCommand(string LoanId) : ICommand;

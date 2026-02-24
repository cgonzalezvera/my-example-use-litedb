using AppExample.Litedb.Application.Abstractions;

namespace AppExample.Litedb.Application.Commands.AddBook;

public record AddBookCommand(
    string Title,
    string Author,
    int Year,
    int Stock
) : ICommand;

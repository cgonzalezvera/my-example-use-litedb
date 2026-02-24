using AppExample.Litedb.Application.Abstractions;

namespace AppExample.Litedb.Application.Commands.AddBookStock;

public record AddBookStockCommand(string BookId, int Quantity) : ICommand;

using AppExample.Litedb.Application.Abstractions;
using AppExample.Litedb.Domain.Repositories;

namespace AppExample.Litedb.Application.Commands.AddBookStock;

public class AddBookStockCommandHandler(IBookRepository bookRepository)
    : ICommandHandler<AddBookStockCommand>
{
    public void Handle(AddBookStockCommand command)
    {
        var book = bookRepository.GetById(command.BookId)
            ?? throw new InvalidOperationException($"No existe un libro con ID '{command.BookId}'.");

        if (command.Quantity <= 0)
            throw new InvalidOperationException("La cantidad debe ser mayor a 0.");

        book.Stock += command.Quantity;
        bookRepository.Update(book);
        Console.WriteLine($"✔ Stock actualizado. \"{book.Title}\" — Nuevo stock: {book.Stock}");
    }
}

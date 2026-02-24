using AppExample.Litedb.Application.Abstractions;
using AppExample.Litedb.Domain.Entities;
using AppExample.Litedb.Domain.Repositories;

namespace AppExample.Litedb.Application.Commands.AddBook;

public class AddBookCommandHandler(IBookRepository bookRepository)
    : ICommandHandler<AddBookCommand>
{
    public void Handle(AddBookCommand command)
    {
        var book = new Book
        {
            Id = Guid.NewGuid().ToString("N")[..8].ToUpper(),
            Title = command.Title,
            Author = command.Author,
            Year = command.Year,
            Stock = command.Stock
        };
        bookRepository.Add(book);
        Console.WriteLine($"✔ Libro agregado. ID: {book.Id} | \"{book.Title}\" — Stock: {book.Stock}");
    }
}

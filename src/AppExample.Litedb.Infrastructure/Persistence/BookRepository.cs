using AppExample.Litedb.Domain.Entities;
using AppExample.Litedb.Domain.Repositories;
using AppExample.Litedb.Infrastructure.Persistence;

namespace AppExample.Litedb.Infrastructure.Persistence;

public class BookRepository(LiteDbContext context) : IBookRepository
{
    private const string CollectionName = "books";

    public Book? GetById(string id) =>
        context.GetCollection<Book>(CollectionName).FindById(id);

    public IEnumerable<Book> GetAll() =>
        context.GetCollection<Book>(CollectionName).FindAll();

    public void Add(Book book) =>
        context.GetCollection<Book>(CollectionName).Insert(book);

    public void Update(Book book) =>
        context.GetCollection<Book>(CollectionName).Update(book);
}

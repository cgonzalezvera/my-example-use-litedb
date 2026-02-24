using AppExample.Litedb.Domain.Entities;

namespace AppExample.Litedb.Domain.Repositories;

public interface IBookRepository
{
    Book? GetById(string id);
    IEnumerable<Book> GetAll();
    void Add(Book book);
    void Update(Book book);
}

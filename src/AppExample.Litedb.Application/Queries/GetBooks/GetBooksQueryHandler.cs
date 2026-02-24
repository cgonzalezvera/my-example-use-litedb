using AppExample.Litedb.Application.Abstractions;
using AppExample.Litedb.Domain.Repositories;

namespace AppExample.Litedb.Application.Queries.GetBooks;

public class GetBooksQueryHandler(IBookRepository bookRepository)
    : IQueryHandler<GetBooksQuery, IEnumerable<BookDto>>
{
    public IEnumerable<BookDto> Handle(GetBooksQuery query) =>
        bookRepository.GetAll()
            .Select(b => new BookDto(b.Id, b.Title, b.Author, b.Year, b.Stock));
}

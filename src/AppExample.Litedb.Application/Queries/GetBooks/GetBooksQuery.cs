using AppExample.Litedb.Application.Abstractions;

namespace AppExample.Litedb.Application.Queries.GetBooks;

public record GetBooksQuery : IQuery<IEnumerable<BookDto>>;

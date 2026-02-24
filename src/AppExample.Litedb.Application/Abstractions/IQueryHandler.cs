namespace AppExample.Litedb.Application.Abstractions;

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    TResult Handle(TQuery query);
}

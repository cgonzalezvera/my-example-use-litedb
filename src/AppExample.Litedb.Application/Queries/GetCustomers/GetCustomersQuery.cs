using AppExample.Litedb.Application.Abstractions;

namespace AppExample.Litedb.Application.Queries.GetCustomers;

public record GetCustomersQuery : IQuery<IEnumerable<CustomerDto>>;

using AppExample.Litedb.Application.Abstractions;
using AppExample.Litedb.Domain.Repositories;

namespace AppExample.Litedb.Application.Queries.GetCustomers;

public class GetCustomersQueryHandler(ICustomerRepository customerRepository)
    : IQueryHandler<GetCustomersQuery, IEnumerable<CustomerDto>>
{
    public IEnumerable<CustomerDto> Handle(GetCustomersQuery query) =>
        customerRepository.GetAll()
            .Select(c => new CustomerDto(c.Id, c.Name, c.Email, c.TotalDebt));
}

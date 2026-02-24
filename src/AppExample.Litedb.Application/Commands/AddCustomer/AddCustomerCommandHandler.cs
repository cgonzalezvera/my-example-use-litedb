using AppExample.Litedb.Application.Abstractions;
using AppExample.Litedb.Domain.Entities;
using AppExample.Litedb.Domain.Repositories;

namespace AppExample.Litedb.Application.Commands.AddCustomer;

public class AddCustomerCommandHandler(ICustomerRepository customerRepository)
    : ICommandHandler<AddCustomerCommand>
{
    public void Handle(AddCustomerCommand command)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid().ToString("N")[..8].ToUpper(),
            Name = command.Name,
            Email = command.Email,
            TotalDebt = 0
        };
        customerRepository.Add(customer);
        Console.WriteLine($"✔ Cliente registrado. ID: {customer.Id} | {customer.Name} <{customer.Email}>");
    }
}

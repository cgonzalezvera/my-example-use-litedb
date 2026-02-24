using AppExample.Litedb.Application.Abstractions;

namespace AppExample.Litedb.Application.Commands.AddCustomer;

public record AddCustomerCommand(string Name, string Email) : ICommand;

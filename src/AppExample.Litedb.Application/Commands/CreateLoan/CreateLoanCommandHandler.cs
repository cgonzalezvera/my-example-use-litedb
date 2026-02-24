using AppExample.Litedb.Application.Abstractions;
using AppExample.Litedb.Domain.Entities;
using AppExample.Litedb.Domain.Enums;
using AppExample.Litedb.Domain.Repositories;

namespace AppExample.Litedb.Application.Commands.CreateLoan;

public class CreateLoanCommandHandler(
    IBookRepository bookRepository,
    ICustomerRepository customerRepository,
    ILoanRepository loanRepository)
    : ICommandHandler<CreateLoanCommand>
{
    public void Handle(CreateLoanCommand command)
    {
        var book = bookRepository.GetById(command.BookId)
            ?? throw new InvalidOperationException($"No existe un libro con ID '{command.BookId}'.");

        if (book.Stock <= 0)
            throw new InvalidOperationException($"El libro \"{book.Title}\" no tiene stock disponible.");

        var customer = customerRepository.GetById(command.CustomerId)
            ?? throw new InvalidOperationException($"No existe un cliente con ID '{command.CustomerId}'.");

        if (command.Days <= 0)
            throw new InvalidOperationException("Los días de préstamo deben ser mayor a 0.");

        var now = DateTime.Today;
        var loan = new Loan
        {
            Id = Guid.NewGuid().ToString("N")[..8].ToUpper(),
            BookId = book.Id,
            CustomerId = customer.Id,
            StartDate = now,
            EndDate = now.AddDays(command.Days),
            Status = LoanStatus.Active
        };

        book.Stock--;
        bookRepository.Update(book);
        loanRepository.Add(loan);

        Console.WriteLine($"✔ Préstamo creado. ID: {loan.Id}");
        Console.WriteLine($"  Libro:    [{book.Id}] \"{book.Title}\"  (stock restante: {book.Stock})");
        Console.WriteLine($"  Cliente:  [{customer.Id}] {customer.Name}");
        Console.WriteLine($"  Inicio:   {loan.StartDate:dd/MM/yyyy}  |  Vence: {loan.EndDate:dd/MM/yyyy}");
    }
}

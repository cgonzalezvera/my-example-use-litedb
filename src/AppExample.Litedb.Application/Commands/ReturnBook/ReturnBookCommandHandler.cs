using AppExample.Litedb.Application.Abstractions;
using AppExample.Litedb.Domain.Entities;
using AppExample.Litedb.Domain.Enums;
using AppExample.Litedb.Domain.Repositories;

namespace AppExample.Litedb.Application.Commands.ReturnBook;

public class ReturnBookCommandHandler(
    ILoanRepository loanRepository,
    IBookRepository bookRepository,
    ICustomerRepository customerRepository,
    IFineRepository fineRepository)
    : ICommandHandler<ReturnBookCommand>
{
    private const decimal FinePerDay = 1.00m;

    public void Handle(ReturnBookCommand command)
    {
        var loan = loanRepository.GetById(command.LoanId)
            ?? throw new InvalidOperationException($"No existe un préstamo con ID '{command.LoanId}'.");

        if (loan.Status != LoanStatus.Active)
            throw new InvalidOperationException($"El préstamo '{command.LoanId}' ya fue devuelto.");

        var returnDate = DateTime.Today;
        var daysLate = (returnDate - loan.EndDate).Days;

        loan.ReturnDate = returnDate;
        loan.Status = daysLate > 0 ? LoanStatus.ReturnedLate : LoanStatus.Returned;
        loanRepository.Update(loan);

        var book = bookRepository.GetById(loan.BookId)!;
        book.Stock++;
        bookRepository.Update(book);

        Console.WriteLine($"✔ Libro devuelto. Préstamo: {loan.Id}  |  \"{book.Title}\"  (stock: {book.Stock})");

        if (daysLate > 0)
        {
            var amount = daysLate * FinePerDay;
            var fine = new Fine
            {
                Id = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                LoanId = loan.Id,
                CustomerId = loan.CustomerId,
                Amount = amount,
                DaysLate = daysLate,
                CreatedAt = returnDate
            };
            fineRepository.Add(fine);

            var customer = customerRepository.GetById(loan.CustomerId)!;
            customer.TotalDebt += amount;
            customerRepository.Update(customer);

            Console.WriteLine($"⚠  Devolución fuera de término: {daysLate} día(s) de retraso.");
            Console.WriteLine($"   Multa generada: ${amount:F2}  |  Deuda total del cliente: ${customer.TotalDebt:F2}");
        }
        else
        {
            Console.WriteLine("   Devolución en término. Sin multa.");
        }
    }
}

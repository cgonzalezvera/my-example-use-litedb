using AppExample.Litedb.Domain.Enums;

namespace AppExample.Litedb.Domain.Entities;

public class Loan
{
    public string Id { get; set; } = null!;
    public string BookId { get; set; } = null!;
    public string CustomerId { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public LoanStatus Status { get; set; }
}

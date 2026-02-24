namespace AppExample.Litedb.Domain.Entities;

public class Fine
{
    public string Id { get; set; } = null!;
    public string LoanId { get; set; } = null!;
    public string CustomerId { get; set; } = null!;
    public decimal Amount { get; set; }
    public int DaysLate { get; set; }
    public DateTime CreatedAt { get; set; }
}

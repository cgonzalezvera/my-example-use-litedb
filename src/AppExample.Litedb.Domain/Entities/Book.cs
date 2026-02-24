namespace AppExample.Litedb.Domain.Entities;

public class Book
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Author { get; set; } = null!;
    public int Year { get; set; }
    public int Stock { get; set; }
}

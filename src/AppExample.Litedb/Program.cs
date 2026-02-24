using AppExample.Litedb.Application.Commands.AddBook;
using AppExample.Litedb.Application.Commands.AddBookStock;
using AppExample.Litedb.Application.Commands.AddCustomer;
using AppExample.Litedb.Application.Commands.CreateLoan;
using AppExample.Litedb.Application.Commands.ReturnBook;
using AppExample.Litedb.Application.Queries.GetBooks;
using AppExample.Litedb.Application.Queries.GetCustomers;
using AppExample.Litedb.Application.Queries.GetLoans;
using AppExample.Litedb.Infrastructure.Persistence;

LoadDotEnv();
var dbPath = Environment.GetEnvironmentVariable("DB_PATH")
    ?? @"c:\temp\litedb-ex01\MyBooks.db";

using var dbContext = new LiteDbContext(dbPath);

var bookRepo     = new BookRepository(dbContext);
var customerRepo = new CustomerRepository(dbContext);
var loanRepo     = new LoanRepository(dbContext);
var fineRepo     = new FineRepository(dbContext);

if (args.Length == 0)
{
    PrintHelp();
    return;
}

try
{
    switch (args[0].ToLower())
    {
        case "add-book":
            var title  = GetArg(args, "--title");
            var author = GetArg(args, "--author");
            var year   = int.Parse(GetArg(args, "--year"));
            var stock  = int.Parse(GetArg(args, "--stock", "1"));
            new AddBookCommandHandler(bookRepo)
                .Handle(new AddBookCommand(title, author, year, stock));
            break;

        case "add-stock":
            var bookId   = GetArg(args, "--book-id");
            var quantity = int.Parse(GetArg(args, "--quantity"));
            new AddBookStockCommandHandler(bookRepo)
                .Handle(new AddBookStockCommand(bookId, quantity));
            break;

        case "add-customer":
            var name  = GetArg(args, "--name");
            var email = GetArg(args, "--email");
            new AddCustomerCommandHandler(customerRepo)
                .Handle(new AddCustomerCommand(name, email));
            break;

        case "loan":
            var loanBookId      = GetArg(args, "--book-id");
            var loanCustomerId  = GetArg(args, "--customer-id");
            var days            = int.Parse(GetArg(args, "--days"));
            new CreateLoanCommandHandler(bookRepo, customerRepo, loanRepo)
                .Handle(new CreateLoanCommand(loanBookId, loanCustomerId, days));
            break;

        case "return":
            var loanId = GetArg(args, "--loan-id");
            new ReturnBookCommandHandler(loanRepo, bookRepo, customerRepo, fineRepo)
                .Handle(new ReturnBookCommand(loanId));
            break;

        case "list-books":
            var books = new GetBooksQueryHandler(bookRepo).Handle(new GetBooksQuery());
            PrintBooks(books);
            break;

        case "list-customers":
            var customers = new GetCustomersQueryHandler(customerRepo).Handle(new GetCustomersQuery());
            PrintCustomers(customers);
            break;

        case "list-loans":
            var activeOnly = HasFlag(args, "--active");
            var loans = new GetLoansQueryHandler(loanRepo, bookRepo, customerRepo)
                .Handle(new GetLoansQuery(activeOnly));
            PrintLoans(loans);
            break;

        default:
            Console.WriteLine($"Comando desconocido: '{args[0]}'");
            PrintHelp();
            break;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"✖ Error: {ex.Message}");
}

// ── Helpers ──────────────────────────────────────────────────────────────────

static void LoadDotEnv()
{
    // Busca .env en el directorio de trabajo y luego en el directorio del ejecutable
    var candidates = new[]
    {
        Path.Combine(Directory.GetCurrentDirectory(), ".env"),
        Path.Combine(AppContext.BaseDirectory, ".env")
    };

    var envFile = candidates.FirstOrDefault(File.Exists);
    if (envFile is null) return;

    foreach (var line in File.ReadAllLines(envFile))
    {
        var trimmed = line.Trim();
        if (trimmed.StartsWith('#') || !trimmed.Contains('=')) continue;

        var separator = trimmed.IndexOf('=');
        var key   = trimmed[..separator].Trim();
        var value = trimmed[(separator + 1)..].Trim();

        // No sobreescribir variables ya definidas en el entorno real
        if (Environment.GetEnvironmentVariable(key) is null)
            Environment.SetEnvironmentVariable(key, value);
    }
}

static string GetArg(string[] args, string flag, string? defaultValue = null)
{
    var idx = Array.IndexOf(args, flag);
    if (idx >= 0 && idx + 1 < args.Length)
        return args[idx + 1];
    if (defaultValue != null)
        return defaultValue;
    throw new ArgumentException($"Falta el argumento requerido: {flag}");
}

static bool HasFlag(string[] args, string flag) =>
    Array.IndexOf(args, flag) >= 0;

// ── Output formatters ────────────────────────────────────────────────────────

static void PrintBooks(IEnumerable<BookDto> books)
{
    var list = books.ToList();
    if (list.Count == 0) { Console.WriteLine("No hay libros registrados."); return; }
    Console.WriteLine($"\n{"ID",-10} {"Título",-40} {"Autor",-25} {"Año",-6} {"Stock",-6}");
    Console.WriteLine(new string('-', 92));
    foreach (var b in list)
        Console.WriteLine($"{b.Id,-10} {b.Title,-40} {b.Author,-25} {b.Year,-6} {b.Stock,-6}");
}

static void PrintCustomers(IEnumerable<CustomerDto> customers)
{
    var list = customers.ToList();
    if (list.Count == 0) { Console.WriteLine("No hay clientes registrados."); return; }
    Console.WriteLine($"\n{"ID",-10} {"Nombre",-30} {"Email",-35} {"Deuda",-10}");
    Console.WriteLine(new string('-', 90));
    foreach (var c in list)
        Console.WriteLine($"{c.Id,-10} {c.Name,-30} {c.Email,-35} ${c.TotalDebt:F2}");
}

static void PrintLoans(IEnumerable<LoanDto> loans)
{
    var list = loans.ToList();
    if (list.Count == 0) { Console.WriteLine("No hay préstamos."); return; }
    Console.WriteLine($"\n{"ID",-10} {"Libro",-30} {"Cliente",-25} {"Inicio",-12} {"Vence",-12} {"Devuelto",-12} {"Estado",-14}");
    Console.WriteLine(new string('-', 120));
    foreach (var l in list)
        Console.WriteLine($"{l.Id,-10} {l.BookTitle,-30} {l.CustomerName,-25} {l.StartDate:dd/MM/yyyy}  {l.EndDate:dd/MM/yyyy}  {(l.ReturnDate.HasValue ? l.ReturnDate.Value.ToString("dd/MM/yyyy") : "-"),-12} {l.Status}");
}

static void PrintHelp()
{
    Console.WriteLine("""

    📚 Sistema de Préstamo de Libros
    ─────────────────────────────────────────────────────────
    Comandos disponibles:

      add-book      --title <título> --author <autor> --year <año> --stock <n>
      add-stock     --book-id <id> --quantity <n>
      add-customer  --name <nombre> --email <email>
      loan          --book-id <id> --customer-id <id> --days <n>
      return        --loan-id <id>
      list-books
      list-customers
      list-loans    [--active]

    Ejemplos:
      dotnet run -- add-book --title "El Hobbit" --author "Tolkien" --year 1937 --stock 3
      dotnet run -- add-customer --name "Juan Pérez" --email "juan@mail.com"
      dotnet run -- loan --book-id ABC123 --customer-id DEF456 --days 14
      dotnet run -- return --loan-id GHI789
      dotnet run -- list-loans --active
    """);
}

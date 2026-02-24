# 📚 Sistema de Préstamo de Libros

Aplicación de consola para gestionar el préstamo de libros a clientes, con control de stock, multas por devolución tardía y acumulación de deuda.

---

## Stack Tecnológico

| Componente | Tecnología |
|---|---|
| Lenguaje | C# 12 |
| Runtime | .NET 8 |
| Base de datos | [LiteDB 5.0.21](https://www.litedb.org/) (embebida, sin servidor) |
| Persistencia | Archivo local: `c:\temp\litedb-ex01\MyBooks.db` |
| Patrón de diseño | Clean Architecture + CQRS (manual, sin MediatR) |

---

## Arquitectura

El proyecto sigue **Clean Architecture** dividido en 4 capas con dependencias unidireccionales:

```
AppExample.Litedb.sln
├── AppExample.Litedb/                        ← Consola (entry point)
└── src/
    ├── AppExample.Litedb.Domain/             ← Entidades + Interfaces
    ├── AppExample.Litedb.Application/        ← Commands, Queries, Handlers (CQRS)
    └── AppExample.Litedb.Infrastructure/     ← Repositorios LiteDB
```

### Dependencias entre capas

```
Console  →  Application  →  Domain
                ↑
         Infrastructure  →  Domain
```

La capa **Domain** no depende de ninguna otra. **Infrastructure** e **Application** conocen al Domain. La **Consola** instancia manualmente los handlers con sus dependencias (sin contenedor DI externo).

---

### Domain (`AppExample.Litedb.Domain`)

Contiene las entidades del negocio y las interfaces de repositorios.

#### Entidades

| Entidad | Descripción |
|---|---|
| `Book` | Libro con Id, Title, Author, Year, Stock |
| `Customer` | Cliente con Id, Name, Email, TotalDebt |
| `Loan` | Préstamo con BookId, CustomerId, StartDate, EndDate, ReturnDate?, Status |
| `Fine` | Multa con LoanId, CustomerId, Amount, DaysLate, CreatedAt |

#### Enum `LoanStatus`
```
Active        → Préstamo en curso
Returned      → Devuelto en término
ReturnedLate  → Devuelto con retraso (multa generada)
```

#### Interfaces de repositorios
- `IBookRepository`
- `ICustomerRepository`
- `ILoanRepository`
- `IFineRepository`

---

### Application (`AppExample.Litedb.Application`)

Implementa **CQRS** (Command Query Responsibility Segregation) con abstracciones propias:

```csharp
ICommand
ICommandHandler<TCommand>
IQuery<TResult>
IQueryHandler<TQuery, TResult>
```

#### Commands

| Command | Descripción |
|---|---|
| `AddBookCommand` | Registra un nuevo libro con stock inicial |
| `AddBookStockCommand` | Incrementa el stock de un libro existente |
| `AddCustomerCommand` | Registra un nuevo cliente, genera ID automático |
| `CreateLoanCommand` | Crea un préstamo (valida stock, genera fechas, reduce stock) |
| `ReturnBookCommand` | Procesa la devolución, recupera stock, genera multa si hay retraso |

#### Queries

| Query | Retorna |
|---|---|
| `GetBooksQuery` | `IEnumerable<BookDto>` — libros con stock |
| `GetCustomersQuery` | `IEnumerable<CustomerDto>` — clientes con deuda |
| `GetLoansQuery(ActiveOnly)` | `IEnumerable<LoanDto>` — préstamos (todos o solo activos) |

---

### Infrastructure (`AppExample.Litedb.Infrastructure`)

Implementa los repositorios usando **LiteDB**:

- `LiteDbContext` — wrapper del `LiteDatabase`, maneja el ciclo de vida
- `BookRepository`
- `CustomerRepository`
- `LoanRepository`
- `FineRepository`

---

### Console (`AppExample.Litedb`)

Entry point de la aplicación. Parsea los argumentos de línea de comandos e instancia manualmente los handlers de Application con sus dependencias de Infrastructure.

---

## Uso

### Requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) o superior
- El directorio `c:\temp\litedb-ex01\` es creado automáticamente al primer uso

### Compilar

```bash
dotnet build
```

### Ejecutar comandos

```bash
dotnet run --project AppExample.Litedb -- <comando> [opciones]
```

---

## Comandos

### `add-book` — Agregar un libro

```bash
dotnet run --project AppExample.Litedb -- add-book --title <título> --author <autor> --year <año> --stock <cantidad>
```

**Ejemplo:**
```bash
dotnet run --project AppExample.Litedb -- add-book --title "El Hobbit" --author "J.R.R. Tolkien" --year 1937 --stock 3
```

**Salida:**
```
✔ Libro agregado. ID: D323953F | "El Hobbit" — Stock: 3
```

---

### `add-stock` — Agregar stock a un libro

```bash
dotnet run --project AppExample.Litedb -- add-stock --book-id <id> --quantity <cantidad>
```

**Ejemplo:**
```bash
dotnet run --project AppExample.Litedb -- add-stock --book-id D323953F --quantity 5
```

**Salida:**
```
✔ Stock actualizado. "El Hobbit" — Nuevo stock: 8
```

---

### `add-customer` — Registrar un cliente

```bash
dotnet run --project AppExample.Litedb -- add-customer --name <nombre> --email <email>
```

**Ejemplo:**
```bash
dotnet run --project AppExample.Litedb -- add-customer --name "Juan Pérez" --email "juan@mail.com"
```

**Salida:**
```
✔ Cliente registrado. ID: C1E2D825 | Juan Pérez <juan@mail.com>
```

---

### `loan` — Registrar un préstamo

Presta un libro a un cliente por N días. Reduce el stock del libro en 1.

```bash
dotnet run --project AppExample.Litedb -- loan --book-id <id> --customer-id <id> --days <días>
```

**Ejemplo:**
```bash
dotnet run --project AppExample.Litedb -- loan --book-id D323953F --customer-id C1E2D825 --days 14
```

**Salida:**
```
✔ Préstamo creado. ID: 66DB7996
  Libro:    [D323953F] "El Hobbit"  (stock restante: 2)
  Cliente:  [C1E2D825] Juan Pérez
  Inicio:   24/02/2026  |  Vence: 10/03/2026
```

> ⚠ Si el libro no tiene stock disponible, el préstamo es rechazado.

---

### `return` — Devolver un libro

Registra la devolución. Recupera el stock del libro. Si la devolución es posterior a la fecha de vencimiento, se genera una **multa de $1 por día de retraso** que se acumula como deuda del cliente.

```bash
dotnet run --project AppExample.Litedb -- return --loan-id <id>
```

**Ejemplo (en término):**
```bash
dotnet run --project AppExample.Litedb -- return --loan-id 66DB7996
```
```
✔ Libro devuelto. Préstamo: 66DB7996  |  "El Hobbit"  (stock: 3)
   Devolución en término. Sin multa.
```

**Ejemplo (con retraso):**
```
✔ Libro devuelto. Préstamo: 66DB7996  |  "El Hobbit"  (stock: 3)
⚠  Devolución fuera de término: 5 día(s) de retraso.
   Multa generada: $5.00  |  Deuda total del cliente: $5.00
```

---

### `list-books` — Listar libros

```bash
dotnet run --project AppExample.Litedb -- list-books
```

**Salida:**
```
ID         Título                                   Autor                     Año    Stock
--------------------------------------------------------------------------------------------
A6331313   1984                                     George Orwell             1949   7
D323953F   El Hobbit                                J.R.R. Tolkien            1937   3
```

---

### `list-customers` — Listar clientes

```bash
dotnet run --project AppExample.Litedb -- list-customers
```

**Salida:**
```
ID         Nombre                         Email                               Deuda
------------------------------------------------------------------------------------------
C1E2D825   Juan Pérez                     juan@mail.com                       $0,00
F1C483EA   Maria Lopez                    maria@mail.com                      $5,00
```

---

### `list-loans` — Listar préstamos

```bash
# Todos los préstamos
dotnet run --project AppExample.Litedb -- list-loans

# Solo préstamos activos
dotnet run --project AppExample.Litedb -- list-loans --active
```

**Salida:**
```
ID         Libro                          Cliente                   Inicio       Vence        Devuelto     Estado
------------------------------------------------------------------------------------------------------------------------
66DB7996   El Hobbit                      Juan Pérez                24/02/2026  10/03/2026  -            Active
```

---

## Flujo típico

```bash
# 1. Agregar libros
dotnet run --project AppExample.Litedb -- add-book --title "Clean Code" --author "Robert Martin" --year 2008 --stock 2

# 2. Registrar un cliente
dotnet run --project AppExample.Litedb -- add-customer --name "Ana García" --email "ana@mail.com"

# 3. Consultar IDs
dotnet run --project AppExample.Litedb -- list-books
dotnet run --project AppExample.Litedb -- list-customers

# 4. Hacer un préstamo (10 días)
dotnet run --project AppExample.Litedb -- loan --book-id <BOOK_ID> --customer-id <CUSTOMER_ID> --days 10

# 5. Ver préstamos activos
dotnet run --project AppExample.Litedb -- list-loans --active

# 6. Devolver el libro
dotnet run --project AppExample.Litedb -- return --loan-id <LOAN_ID>
```

---

## Reglas de negocio

- Un préstamo reduce el stock del libro en 1. Si el stock es 0, el préstamo es rechazado.
- La devolución recupera el stock en 1 unidad.
- Si la fecha de devolución es posterior a `EndDate`, se genera una multa de **$1 por día de retraso**.
- Las multas se acumulan en el campo `TotalDebt` del cliente.
- Un préstamo ya devuelto no puede procesarse nuevamente.
- Los IDs de libros, clientes y préstamos son generados automáticamente (8 caracteres hex en mayúscula).

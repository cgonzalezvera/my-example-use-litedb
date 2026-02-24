using LiteDB;

namespace AppExample.Litedb.Infrastructure.Persistence;

public class LiteDbContext : IDisposable
{
    private readonly LiteDatabase _db;

    public LiteDbContext(string databasePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(databasePath)!);
        _db = new LiteDatabase(databasePath);
    }

    public ILiteCollection<T> GetCollection<T>(string name) => _db.GetCollection<T>(name);

    public void Dispose() => _db.Dispose();
}

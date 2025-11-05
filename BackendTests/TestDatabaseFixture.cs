using Microsoft.EntityFrameworkCore;
using DataServiceLayer;

namespace Backend.Tests;

public class TestDatabaseFixture
{
    private const string ConnectionString = "Host=localhost;Database=imdb_test;Username=postgres;Password=12345";

    public IDataService CreateDataService()
    {
        return new DataService(ConnectionString);
    }

    public ImdbContext CreateContext()
    {
        return new ImdbContext(ConnectionString);
    }

    public void Initialize()
    {
        using var context = CreateContext();
        context.Database.EnsureCreated();
    }

    public void Cleanup()
    {
        using var context = CreateContext();
        context.Database.EnsureDeleted();
    }
}
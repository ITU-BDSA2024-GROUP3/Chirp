using Chirp.Razor;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTest;

public class UtilFunctionsTest
{
    public static async Task<ICheepRepository> CreateInMemoryDb()
    {
        using var connection = new SqliteConnection("DefaultConnection=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database

        return new CheepRepository(context);

// Act
        //var result = repository.ReadCheeps(1, 1);
        
    }
}
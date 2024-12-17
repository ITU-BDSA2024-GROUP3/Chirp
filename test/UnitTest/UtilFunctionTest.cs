using ChirpCore;
using ChirpCore.DomainModel;
using ChirpInfrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTest;

public class UtilFunctionsTest
{
    public static async Task<ICheepRepository> CreateInMemoryDb()
    {
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
        
        var author = new Author() { UserId = 1, Cheeps = null, Email = "mymail", Name = "Tom", FollowingList = new List<int>()};
        
        var cheep = new Cheep(
            cheepId: 1,
            text: "messageData",
            timeStamp: DateTimeOffset.FromUnixTimeSeconds(1728643569).UtcDateTime, // Ensure this matches the format in your model
            author: author,
            userId: author.UserId,
            authorLikeList: new List<int>()
        );

        context.Authors.Add(author);
        context.Cheeps.Add(cheep);
        await context.SaveChangesAsync();
        return new CheepRepository(context);

        // Act
        //var result = repository.ReadCheeps(1, 1);
        
    }
}
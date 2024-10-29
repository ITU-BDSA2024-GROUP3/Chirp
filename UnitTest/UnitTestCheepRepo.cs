using ChirpCore;
using ChirpCore.DomainModel;
using ChirpInfrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTest;

public class UnitTestCheepRepo : IDisposable
{
   

    public async void Dispose()
    {
    }

    [Fact]
    public async void TestReadCheep()
    {
        //var repo = await UtilFunctionsTest.CreateInMemoryDb();
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
        
        var author = new Author() { AuthorId = 1, Cheeps = null, Email = "mymail", Name = "Tom" };
        
        var cheep = new Cheep
        {
            CheepId = 1,
            Author = author,
            AuthorId = author.AuthorId,
            Text = "messageData",
            TimeStamp = DateTimeOffset.FromUnixTimeSeconds(1728643569).UtcDateTime // Ensure this matches the format in your model
        };

        context.Authors.Add(author);
        context.Cheeps.Add(cheep);
        await context.SaveChangesAsync();
        ICheepRepository repo = new CheepRepository(context);
        var cheeps = await repo.ReadCheeps(1, null);
        Assert.NotNull(cheeps);
        var messages = new List<string>();
        foreach (CheepDTO cheeP in cheeps)
        {
            messages.Add(cheeP.Text);
        }
        Assert.True(messages.Contains("messageData"));
    }
    /*[Theory]
    public async void TestReadCheepText()
    {

    }*/
   
    
}
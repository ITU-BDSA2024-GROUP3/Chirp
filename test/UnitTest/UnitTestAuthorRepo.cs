using ChirpCore;
using ChirpCore.DomainModel;
using ChirpInfrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTest;

public class UnitTestAuthorRepo
{
    [Theory]
    [InlineData(1, "Tom", "myemail")]
    public async void TestReadAuthorById(int id, string name, string email)
    {
        //var repo = await UtilFunctionsTest.CreateInMemoryDb();
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database

        var author = new Author() { UserId = id, Cheeps = null, Email = email, Name = name, FollowingList = new List<int>()};

        context.Authors.Add(author);
        await context.SaveChangesAsync();
        IAuthorRepository repo = new AuthorRepository(context);

        var authorDto = await repo.ReadAuthorDTOById(id);
        Assert.NotNull(author);
        Assert.Equal(name, authorDto.Name);
    }
    [Theory]
    [InlineData(1, "Tom", "myemail")]
    [InlineData(1, "Tom", "Tom")] //should not pass
    public async void TestReadAuthorByEmail(int id, string name, string email)
    {
        //var repo = await UtilFunctionsTest.CreateInMemoryDb();
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database

        var author = new Author() { UserId = id, Cheeps = null, Email = email, Name = name, FollowingList = new List<int>()};

        context.Authors.Add(author);
        await context.SaveChangesAsync();
        IAuthorRepository repo = new AuthorRepository(context);

        var authorDto = await repo.ReadAuthorByEmail(email);
        Assert.NotNull(author);
        Assert.Equal(name, authorDto.Name);
    }
    [Theory]
    [InlineData(1, "Tom", "myemail")]
    public async void TestReadAuthorByName(int id, string name, string email)
    {
        //var repo = await UtilFunctionsTest.CreateInMemoryDb();
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database

        var author = new Author() { UserId = id, Cheeps = null, Email = email, Name = name, FollowingList = new List<int>()};

        context.Authors.Add(author);
        await context.SaveChangesAsync();
        IAuthorRepository repo = new AuthorRepository(context);

        var authorDto = await repo.ReadAuthorByName(name);
        Assert.NotNull(author);
        Assert.Equal(name, authorDto.Name);
    }
    [Theory]
    [InlineData(1, "Tom", "myemail")]
    public async void TestCreateAuthor(int id, string name, string email)
    {
        //var repo = await UtilFunctionsTest.CreateInMemoryDb();
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database

        IAuthorRepository repo = new AuthorRepository(context);
        var author = await repo.CreateAuthor(name, email, id);

        Assert.NotNull(author);

    }
    [Theory]
    [InlineData(1, "Tom", "myemail", 10)]
    [InlineData(2, "Tommy", "myemail", 1)]
    public async void TestGetAuthorCount(int id, string name, string email, int authorNr)
    {
        //var repo = await UtilFunctionsTest.CreateInMemoryDb();
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database

        IAuthorRepository repo = new AuthorRepository(context);

        for (int i = 0; i < authorNr; i++)
        {
            await repo.CreateAuthor(name + i, email + i, id + i);
        }

        var athcount = await repo.GetAuthorCount();

        Assert.Equal(authorNr, athcount);
    }
    [Fact]
    public async void TestGetNameByEmail()
    {
        //var repo = await UtilFunctionsTest.CreateInMemoryDb();
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
      
        //create author and add to database
        var author = new Author() { UserId= 1, Cheeps = null, Email = "mymail", Name = "Tom", FollowingList = new List<int>() };
        context.Authors.Add(author);
        await context.SaveChangesAsync();
      
        IAuthorRepository repo = new AuthorRepository(context);
      
        Assert.Equal("Tom", await repo.GetNameByEmail("mymail"));
        Assert.Null(await repo.GetNameByEmail("notmail"));
    }
    [Fact]
    public async void followAndUnFolloweAuthor()
    {
        using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

        using var context = new ChirpDBContext(builder.Options);
        await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
      
        //create author and add to database
        var author1 = new Author() { UserId= 1, Cheeps = null, Email = "mymail", Name = "Tom", FollowingList = new List<int>() };
        context.Authors.Add(author1);
      
        var author2 = new Author() { UserId= 2, Cheeps = null, Email = "mymaile", Name = "Tommy", FollowingList = new List<int>() };
        context.Authors.Add(author2);
      
        await context.SaveChangesAsync();
      
        IAuthorRepository repo = new AuthorRepository(context);
      
        Assert.Equal(1,repo.Follow(author1.UserId,author2.UserId).Result);
        Assert.ThrowsAsync<AggregateException> ( () => repo.Follow(author1.UserId,author2.UserId));
        Assert.ThrowsAsync<Exception>(() => repo.Unfollow(author1.UserId,author2.UserId));
    }
}

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

      var author = new Author() { UserId = 1, Cheeps = null, Email = "mymail", Name = "Tom" };

      var cheep = new Cheep
      {
         CheepId = 1,
         Author = author,
         UserId = author.UserId,
         Text = "messageData",
         TimeStamp = DateTimeOffset.FromUnixTimeSeconds(1728643569)
            .UtcDateTime // Ensure this matches the format in your model
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

      //Assert.True(messages.Contains("messageData"));
      Assert.Contains("messageData", messages);
   }

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

      var author = new Author() { UserId = id, Cheeps = null, Email = email, Name = name };

      context.Authors.Add(author);
      await context.SaveChangesAsync();
      ICheepRepository repo = new CheepRepository(context);

      var authorDto = await repo.ReadAuthorDTOById(id);
      Assert.NotNull(author);
      Assert.Equal(name, authorDto.Name);
   }

   [Theory]
   [InlineData(1, "Tom", "myemail")]
   public async void TestReadAuthorByEmail(int id, string name, string email)
   {
      //var repo = await UtilFunctionsTest.CreateInMemoryDb();
      using var connection = new SqliteConnection("Filename=:memory:");
      await connection.OpenAsync();
      var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

      using var context = new ChirpDBContext(builder.Options);
      await context.Database.EnsureCreatedAsync(); // Applies the schema to the database

      var author = new Author() { UserId = id, Cheeps = null, Email = email, Name = name };

      context.Authors.Add(author);
      await context.SaveChangesAsync();
      ICheepRepository repo = new CheepRepository(context);

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

      var author = new Author() { UserId = id, Cheeps = null, Email = email, Name = name };

      context.Authors.Add(author);
      await context.SaveChangesAsync();
      ICheepRepository repo = new CheepRepository(context);

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

      ICheepRepository repo = new CheepRepository(context);
      var author = await repo.CreateAuthor(name, email, id);

      Assert.NotNull(author);

   }

   [Theory]
   [InlineData(1, "Tom", "myemail", 10)]
   public async void TestGetAuthorCount(int id, string name, string email, int authorNr)
   {
      //var repo = await UtilFunctionsTest.CreateInMemoryDb();
      using var connection = new SqliteConnection("Filename=:memory:");
      await connection.OpenAsync();
      var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

      using var context = new ChirpDBContext(builder.Options);
      await context.Database.EnsureCreatedAsync(); // Applies the schema to the database



      ICheepRepository repo = new CheepRepository(context);

      for (int i = 0; i < authorNr; i++)
      {
         await repo.CreateAuthor(name + i, email + i, id + i);
      }

      var athcount = await repo.GetAuthorCount();

      Assert.Equal(authorNr, athcount);
   }

   [Fact]
   public async void TestCreateCheep()
   {
      //var repo = await UtilFunctionsTest.CreateInMemoryDb();
      using var connection = new SqliteConnection("Filename=:memory:");
      await connection.OpenAsync();
      var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

      using var context = new ChirpDBContext(builder.Options);
      await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
      
      //create author and add to database
      var author = new Author() { UserId= 1, Cheeps = null, Email = "mymail", Name = "Tom" };
      context.Authors.Add(author);
      await context.SaveChangesAsync();
      //MAKE THE DATABASE WITH AUTHOR
      ICheepRepository repo = new CheepRepository(context);
      
      //dto cheep that will be created by our author
      var cheepDataTransferObject = new CheepDTO
      {
         Text = "Something",
         AuthorID = author.UserId,
         AuthorName = author.Name,
         TimeStamp = 1728643569
      };
      
      var result = await repo.CreateCheep(cheepDataTransferObject);
      Assert.Equal(1, result);
   }

   [Fact]
   public async void TestUpdateCheep()
   {
      //var repo = await UtilFunctionsTest.CreateInMemoryDb();
      using var connection = new SqliteConnection("Filename=:memory:");
      await connection.OpenAsync();
      var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

      using var context = new ChirpDBContext(builder.Options);
      await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
      
      //create author and add to database
      var author = new Author() { UserId= 1, Cheeps = null, Email = "mymail", Name = "Tom" };
      context.Authors.Add(author);
      await context.SaveChangesAsync();
      //MAKE THE DATABASE WITH AUTHOR
      ICheepRepository repo = new CheepRepository(context);
      
      var c1 = new Cheep()
      {
         CheepId = 1, 
         UserId = author.UserId,
         Author = author, 
         Text = "The two went past me.", 
         TimeStamp = DateTime.Parse("2023-08-01 13:14:37")
      };
      
      context.Cheeps.Add(c1);
      await context.SaveChangesAsync();
      Assert.Equal(1, await repo.UpdateCheep(c1));
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
      var author = new Author() { UserId= 1, Cheeps = null, Email = "mymail", Name = "Tom" };
      context.Authors.Add(author);
      await context.SaveChangesAsync();
      
      ICheepRepository repo = new CheepRepository(context);
      
      Assert.Equal("Tom", await repo.GetNameByEmail("mymail"));
      Assert.Null(await repo.GetNameByEmail("notmail"));
   }




/*
[Theory]
public async void TestReadCheepText()
{

}

*/
   
    
}
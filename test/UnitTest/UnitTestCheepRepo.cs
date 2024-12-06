
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

   public async Task<ChirpDBContext> CreateDbContext()
   {
      var connection = new SqliteConnection("Filename=:memory:");
      await connection.OpenAsync();
      var builder = new DbContextOptionsBuilder<ChirpDBContext>().UseSqlite(connection);

      var context = new ChirpDBContext(builder.Options);
      await context.Database.EnsureCreatedAsync(); // Applies the schema to the database
      return context;
   }

   public async void AddCheepOne(ChirpDBContext context, Author author)
   {
      var cheep = new Cheep
      {
         CheepId = 1,
         Author = author,
         UserId = author.UserId,
         Text = "messageData",
         TimeStamp = DateTimeOffset.FromUnixTimeSeconds(1728643569).UtcDateTime // Ensure this matches the format in your model

      };
      context.Cheeps.Add(cheep);

      
   }
   public async Task<ICheepRepository> CreateInMemoryDb()
   {
      var context = await CreateDbContext();
        
      var author = new Author() { UserId = 1, Cheeps = null, Email = "mymail", Name = "Tom", FollowingList = new List<int>()};
        
      AddCheepOne(context, author);

      context.Authors.Add(author);
      await context.SaveChangesAsync();
      return new CheepRepository(context);

      // Act
      //var result = repository.ReadCheeps(1, 1);
        
   }
   [Fact]
   public async void TestReadCheep()
   {
      
      ICheepRepository repo = await CreateInMemoryDb();
      var cheeps = repo.ReadCheeps(1, null);
      Assert.NotNull(cheeps);
      var messages = new List<string>();
      foreach (CheepDTO cheeP in cheeps)
      {
         messages.Add(cheeP.Text);
      }

      //Assert.True(messages.Contains("messageData"));
      Assert.Contains("messageData", messages);
   }
   
   [Fact]
   public async void TestReadfollowerCheep()
   {
      //var repo = await UtilFunctionsTest.CreateInMemoryDb();
      var context = await CreateDbContext();

      var author1 = new Author() { UserId = 1, Cheeps = null, Email = "mymail", Name = "Tom" , FollowingList = new List<int>()};
      var author2 = new Author() { UserId = 2, Cheeps = null, Email = "mymaile", Name = "Tommy" , FollowingList = new List<int>()};
      author1.FollowingList.Add(author2.UserId);
      
      AddCheepOne(context, author2);

      context.Authors.Add(author1);
      context.Authors.Add(author2);
      await context.SaveChangesAsync();
      ICheepRepository repo = new CheepRepository(context);
      var cheeps = repo.ReadFollowedCheeps(1, 1);
      Assert.NotNull(cheeps);
      var messages = new List<string>();
      foreach (CheepDTO cheeP in cheeps)
      {
         messages.Add(cheeP.Text);
      }

      //Assert.True(messages.Contains("messageData"));
      Assert.Contains("messageData", messages);
   }

  

   

   



   

   [Fact]
   public async void TestCreateCheep()
   {
      var context = await CreateDbContext();
      
      //create author and add to database
      var author = new Author() { UserId= 1, Cheeps = null, Email = "mymail", Name = "Tom", FollowingList = new List<int>() };
      context.Authors.Add(author);
      await context.SaveChangesAsync();
      //MAKE THE DATABASE WITH AUTHOR
      ICheepRepository repo = new CheepRepository(context);
      
      //dto cheep that will be created by our author
      var cheepDataTransferObject = new CheepDTO(
         text: "Something",
         userId: author.UserId,
         cheepId: 1,
         authorName: author.Name,
         timeStamp: 1728643569);
      
      var result = await repo.CreateCheep(cheepDataTransferObject);
      Assert.Equal(1, result);
   }

   [Fact]
   public async void TestUpdateCheep()
   {
      var context = await CreateDbContext();
      
      //create author and add to database
      var author = new Author() { UserId= 1, Cheeps = null, Email = "mymail", Name = "Tom", FollowingList = new List<int>()};
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
   public void convertunixtimestamp()
   {
      int timestamp = 1728284672;
      String time = CheepRepository.UnixTimeStampToDateTimeString(timestamp);
      Assert.Equal(time, "10/07/24 7.04.32");
      
        
   }
   
   [Fact]
   public async void likeandunlikecheeps()
   {
      //var context = await CreateDbContext();

      //create author and add to database
      /*var author = new Author() { UserId= 1, Cheeps = null, Email = "mymail", Name = "Tom", FollowingList = new List<int>() };
      context.Authors.Add(author);
      await context.SaveChangesAsync();
      //MAKE THE DATABASE WITH AUTHOR
      ICheepRepository repo = new CheepRepository(context);

      //dto cheep that will be created by our author
      var cheepDataTransferObject = new CheepDTO
      (
         text: "Something",
         userId: author.UserId,
         cheepId: 1,
         authorName: author.Name,
         timeStamp: 1728643569
      );

      await repo.CreateCheep(cheepDataTransferObject);
      Assert.Equal(1,repo.LikeCheep(1, 1).Result);
      Assert.Equal(0,repo.UnLikeCheep(1, 1).Result);*/
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
      //MAKE THE DATABASE WITH AUTHOR
      ICheepRepository repo = new CheepRepository(context);

      //dto cheep that will be created by our author
      var cheepDataTransferObject = new CheepDTO
      (
         text: "Something",
         userId: author.UserId,
         cheepId: 1,
         authorName: author.Name,
         timeStamp: 1728643569
      );

      await repo.CreateCheep(cheepDataTransferObject);
      Assert.Equal(1,repo.LikeCheep(1, 1).Result);
      Assert.Equal(0,repo.UnLikeCheep(1, 1).Result);




   }
   
   
   //add follower, remove follower, readfollowercheeps
   
/*
[Theory]
public async void TestReadCheepText()
{

}

*/
   
    
}
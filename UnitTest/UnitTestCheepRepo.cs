using Chirp.Razor;
using Chirp.Razor.ChirpCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTest;

public class UnitTestCheepRepo
{
   [Fact]
   public async void TestReadCheep()
   {
      var repo = await UtilFunctionsTest.CreateInMemoryDb();
      var cheeps = await repo.ReadCheeps(1, null);
      Assert.NotNull(cheeps);
      var messages = new List<string>();
      foreach (CheepDTO cheep in cheeps)
      {
         messages.Add(cheep.Text);
      }
      Assert.True(messages.Contains("They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me"));
   }
   /*[Theory]
   public async void TestReadCheepText()
   {
      
   }*/
   
    
}
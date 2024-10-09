using Chirp.Razor;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace UnitTest;

public class UnitTestCheepRepo
{
   [Fact]
   public async void TestReadCheep()
   {
      var repo = await UtilFunctionsTest.CreateInMemoryDb();
      var cheep = await repo.ReadCheeps(1, null);
      Assert.NotNull(cheep);
   }
    
}
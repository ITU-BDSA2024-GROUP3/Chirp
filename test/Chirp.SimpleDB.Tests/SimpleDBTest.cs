
namespace Chirp.SimpleDB.Tests;


public class SimpleDBTest : IDisposable
{
    private CSVDatabase<Cheep> cheepManager;
    public SimpleDBTest()
    {
        cheepManager = CSVDatabase<Cheep>.instance;
        cheepManager.setPath("../data/TestData.csv");
    }

    //Tear down
    public void Dispose()
    {
        
    }


        
    [Theory]
    public void TestMessage(string message, long unixTimeStamp)
    {
        
        cheepManager.Store(new Cheep(Environment.UserName, message, unixTimeStamp));
        
        Assert.Equal(message , cheepManager.Read().Last().Message);
        
    }
    [Theory]
    public void TestAuthor(string message, long unixTimeStamp)
    {
        
        cheepManager.Store(new Cheep(Environment.UserName, message, unixTimeStamp));
        
        Assert.Equal(Environment.UserName , cheepManager.Read().Last().Author);
        
    }
    
    [Theory]
    public void TestTimeStamp(string message, long unixTimeStamp)
    {
        
        cheepManager.Store(new Cheep(Environment.UserName, message, unixTimeStamp));
        
        Assert.Equal(unixTimeStamp , cheepManager.Read().Last().Timestamp);
        
    }
}
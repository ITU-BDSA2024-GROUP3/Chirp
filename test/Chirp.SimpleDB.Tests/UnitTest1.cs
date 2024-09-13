namespace Chirp.SimpleDB.Tests;

public class UnitTest1
{
    public string dataPath ="data/TestData.csv";
    
    var cheepManager = new CSVDatabase<Cheep>(dataPath);
        
    [Theory]
    public void TestMessage(string message, long unixTimeStamp)
    {
        
        cheepManager.Store(new Cheep(Environment.UserName, message, unixTimeStamp));
        
        Assert.Equal(message , cheepManager.Read.Last().Message);
        
    }
    
    public void TestAuthor(string message, long unixTimeStamp)
    {
        
        cheepManager.Store(new Cheep(Environment.UserName, message, unixTimeStamp));
        
        Assert.Equal(Environment.UserName , cheepManager.Read.Last().Author);
        
    }
    
    public void TestTimeStamp(string message, long unixTimeStamp)
    {
        
        cheepManager.Store(new Cheep(Environment.UserName, message, unixTimeStamp));
        
        Assert.Equal(unixTimeStamp , cheepManager.Read.Last().Timestamp);
        
    }
}
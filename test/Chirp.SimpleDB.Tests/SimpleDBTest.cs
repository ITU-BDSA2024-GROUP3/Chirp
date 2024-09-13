
using CsvHelper;
using CsvHelper.Configuration;

namespace Chirp.SimpleDB.Tests;


public class SimpleDBTest : IDisposable
{
    private CSVDatabase<Cheep> cheepManager;
    private readonly CsvConfiguration _csvConfig;

    public SimpleDBTest()
    {
        cheepManager = CSVDatabase<Cheep>.instance;
        cheepManager.setPath("../data/TestData.csv");
    }

    //Tear down
    public void Dispose()
    {
        var dataPath = "../data/TestData.csv";
        cheepManager.setPath(dataPath);

        File.WriteAllText(dataPath, String.Empty);
    }

    [Fact]
    public void testException()
    {
        //Act
        cheepManager.setPath("../data/TestDat.csv");
        var exceptionType = typeof(FileNotFoundException);
        
        //Assert
        Assert.Throws(exceptionType, () => {
            throw new FileNotFoundException();
        });
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

using System.Diagnostics;
using CsvHelper;
using CsvHelper.Configuration;

namespace Chirp.SimpleDB.Tests;


public class SimpleDBTest : IDisposable
{
    private CSVDatabase<Cheep> cheepManager;
    private readonly CsvConfiguration _csvConfig;
    private string dataPath = "../../../../data/TestData.csv";

    public SimpleDBTest()
    {
        cheepManager = CSVDatabase<Cheep>.instance;
        cheepManager.setPath(dataPath);
    }

    //Tear down
    public void Dispose()
    {
        cheepManager.setPath(dataPath);

        File.WriteAllText(dataPath, String.Empty);
        using (var writer = new StreamWriter(dataPath, append: true))
        {
            writer.WriteLine("Author,Message,Timestamp");
        }
    }

    [Fact]
    public void readException()
    {
        //Assign
        var exceptionType = typeof(FileNotFoundException);
        
        //Act
        cheepManager.setPath("../data/TestDat.csv");
        
        //Assert
        Assert.Throws(exceptionType, () => { cheepManager.Read(); });
    }

    [Fact]
    public void fileExists()
    {
        //Assign
        
        //Act
        File.Create(dataPath).Close();
        //Assert
        Assert.True(File.Exists(dataPath));
    }

    [Fact]
    public void fileInput()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..","..","..","..","..","..","src","Chirp.CLI");
        
        //Assign
        {
            //add cheep to file then add blank character to end
            csv.WriteRecord(c1);
            writer.WriteLine();
        }

        //Act



        //Assert

    }
    */


        
    [Theory]
    [InlineData("Hello World", 1726177000)]
    [InlineData("æøå", 1726174826)]
    public void TestMessage(string message, long unixTimeStamp)
    {
        
        cheepManager.Store(new Cheep(Environment.UserName, message, unixTimeStamp));
        
        Assert.Equal(message , cheepManager.Read().Last().Message);
        
    }
    [Theory]
    [InlineData("Hello World", 1726177000)]
    [InlineData("æøå", 1726174826)]

    public void TestAuthor(string message, long unixTimeStamp)
    {
        
        cheepManager.Store(new Cheep(Environment.UserName, message, unixTimeStamp));
        
        Assert.Equal(Environment.UserName , cheepManager.Read().Last().Author);
        
    }
    
    [Theory]
    [InlineData("Hello World", 1726177000)]  
    [InlineData("æøå", 1726174826)]
    public void TestTimeStamp(string message, long unixTimeStamp)
    {
        
        cheepManager.Store(new Cheep(Environment.UserName, message, unixTimeStamp));
        
        Assert.Equal(unixTimeStamp , cheepManager.Read().Last().Timestamp);
        
    }

    [Theory]
    [InlineData("Hello World", 1726177000)]  
    [InlineData("æøå", 1726174826)]
    //inspiration from lecture example
    public void TestCheep(string message, long unixTimeStamp)
    {
        Cheep cheep = new Cheep(Environment.UserName, message, unixTimeStamp);
        cheepManager.Store(cheep);
        Assert.Equal(cheep , cheepManager.Read().Last());
    }

    [Theory]
    [InlineData("Hello World")]
    [InlineData("æøå")]
    public void TestCheepCurrentTime(string message)
    {
        Cheep cheep = new Cheep(Environment.UserName, message, ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
        cheepManager.Store(cheep);
        Assert.Equal(cheep, cheepManager.Read().Last());
    }

    [Theory]
    [InlineData("Hello World")]
    [InlineData("æøå")]
    public void TestAuthorCurrentTime(string message)
    {
        Cheep cheep = new Cheep(Environment.UserName, message, ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
        cheepManager.Store(cheep);
        Assert.Equal(Environment.UserName, cheepManager.Read().Last().Author);
    }

    [Theory]
    [InlineData("Hello World")]
    [InlineData("æøå")]
    public void TestMessageCurrentTime(string message)
    {
        
        Cheep cheep = new Cheep(Environment.UserName, message, ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
        cheepManager.Store(cheep);
        Assert.Equal(message, cheepManager.Read().Last().Message);
    }

    [Theory]
    [InlineData("Hello World")]
    [InlineData("æøå")]
    public void TestTimeCurrentTime(string message)
    {
        long time = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
        Cheep cheep = new Cheep(Environment.UserName, message, time);
        cheepManager.Store(cheep);
        Assert.Equal(time, cheepManager.Read().Last().Timestamp);

        
    }
    
    /*[Theory]
    [InlineData("Hello World")]  
    [InlineData("æøå")]
    public void TestCheepWithCreateCheep(string message)
    {
        Cheep cheep = Program.CreateCheep(message);
        cheepManager.Store(cheep);
        Assert.Equal(cheep , cheepManager.Read().Last());

    }*/
    
    
}
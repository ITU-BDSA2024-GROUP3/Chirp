
using System.Diagnostics;
using System.Net.Http.Headers;
using CsvHelper;
using CsvHelper.Configuration;
using Xunit.Abstractions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;


namespace Chirp.SimpleDB.Tests;


public class SimpleDBTest : IDisposable
{
    private readonly ITestOutputHelper _testOutputHelper;
    private CSVDatabase<Cheep> cheepManager;
    private readonly CsvConfiguration _csvConfig;
    private string dataPath = "../../../../../data/TestData.csv";
    private string baseURL = "http://localhost:5132";
    private HttpClient client = new();

    public SimpleDBTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        
        
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);

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
    
    public void InsertCheeps(int amount, string message)
    {
        for (int i = 0; i < amount+10; i++)
        {

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
        
    [Theory]
    [InlineData("Hello World", 1726177000)]
    [InlineData("æøå", 1726174826)]
    public async void TestMessage(string message, long unixTimeStamp)
    {
        var cheep = Util.CreateCheep(message);

        
        var requestURI = $"cheep";
        requestURI += $"?message={message}";
        
    
    
        CancellationTokenSource cts = new();
        CancellationToken cancellationToken = cts.Token;
    
        var temp =  await client.PostAsJsonAsync(requestURI, cheep, cancellationToken);
        
        requestURI = $"cheeps";
        var cheeps = await client.GetFromJsonAsync<IEnumerable<Cheep>>(requestURI);
        
        Assert.Equal(message , cheeps.Last().Message);
        
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

    [Theory]
    [InlineData(10)]
    [InlineData(5)]
    [InlineData(100)]
    [InlineData(48)]

    public void TestReadAmountWithLimit(int limit)
    {
        InsertCheeps(limit, "Hello World");
        int count = 0;
        foreach (var cheep in cheepManager.Read(limit))
        {
            count++;
        }
        Assert.Equal(limit, count);
    }

    [Theory]
    [InlineData("Hello World")]
    [InlineData("åæø")]
    public void TestReadMessage(string message)
    {
        InsertCheeps(10, message);
        foreach (var cheep in cheepManager.Read())
        {
            Assert.Equal(message, cheep.Message);
        }
       
    }
}
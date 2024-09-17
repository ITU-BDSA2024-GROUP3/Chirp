using CsvHelper.Configuration;

namespace Chirp.SimpleDB.Tests;

public class SimpleDBTest : IDisposable
{
    private readonly CsvConfiguration _csvConfig;
    private readonly CSVDatabase<Cheep> cheepManager;
    private readonly string dataPath = "../../../../data/TestData.csv";

    public SimpleDBTest()
    {
        cheepManager = CSVDatabase<Cheep>.instance;
        cheepManager.setPath(dataPath);
    }

    //Tear down
    public void Dispose()
    {
        cheepManager.setPath(dataPath);

        //File.WriteAllText(dataPath, String.Empty);
    }

    [Fact]
    public void TestReadException()
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

    /*[Theory]
    [InlineData(CreateCheep(string message))]
    public void readOutput(Cheep c1)
    {
        //Assign
        using (var writer = new StreamWriter(dataPath, append: true))
        using (var csv = new CsvWriter(writer, _csvConfig))
        {
            //add cheep to file then add blank character to end
            csv.WriteRecord(c1);
            writer.WriteLine();
        }

        //Act



        //Assert

    }
    */


    /*[Theory]
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
    */
}
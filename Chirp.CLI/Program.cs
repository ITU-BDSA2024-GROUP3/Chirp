using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;


//Structure of a cheep corrosponding to headers of CSV file
public record Cheep(string Author, string Message, long Timestamp);


//CheepManager does what it says
public class CheepManager
{
    //idk much about readonly, rider just said it would be good
    private readonly string _dataPath;
    private readonly CsvConfiguration _csvConfig;

    public CheepManager(string dataPath)
    {
        _dataPath = dataPath;
        //set the config to "InvariantCulture" and inform the program that the file already has headers
        _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
    }

    //again, self explanatory
    public void ReadCheeps()
    {
        //ensure file exists
        if (!File.Exists(_dataPath))
        {
            Console.WriteLine("Data file does not exist.");
            return;
        }
        //create streamreader and CSVreader with "using"
        using (var reader = new StreamReader(_dataPath))
        using (var csv = new CsvReader(reader, _csvConfig))
        {
            //assaign a recod of cheeps and loop through them
            var cheeps = csv.GetRecords<Cheep>(); 
            foreach (var cheep in cheeps)
            {
                DateTimeOffset timeOffset = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp);
                string time = $"{timeOffset.Day}/{timeOffset.Month}/{timeOffset.Year}";
                Console.WriteLine($"{cheep.Author} @ {time} : {cheep.Message}");
            }
        }
    }
    
    //here we go again
    public void AddCheep(string message)
    {
        //create new cheep record
        var newCheep = new Cheep(Environment.UserName, message, ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
        
        //create streamwriter and CSVwriter with using
        using (var writer = new StreamWriter(_dataPath, append: true))
        using (var csv = new CsvWriter(writer, _csvConfig))
        {
            //add cheep to file then add blank character to end
            csv.WriteRecord(newCheep);
            writer.WriteLine();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        const string dataPath = "./chirp_cli_db.csv";
        var cheepManager = new CheepManager(dataPath);

        if (args[0] == "read")
        {
            cheepManager.ReadCheeps();
        }
        else if (args[0] == "cheep")
        {
            cheepManager.AddCheep(args[1]);
        }
        else
        {
            Console.WriteLine("Invalid command. Use 'read' to display cheeps or 'cheep <message>' to add a new cheep.");
        }
    }
}

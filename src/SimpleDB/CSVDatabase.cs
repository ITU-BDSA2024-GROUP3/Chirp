using System.Globalization;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.FileProviders;

namespace SimpleDB;

public class CSVDatabase<T> : IDatabaseRepository<T>
{
    private readonly CsvConfiguration _csvConfig;

    private readonly IFileProvider embedded;

    //idk much about readonly, rider just said it would be good
    //private string dataPath = "/../../../../data/chirp_cli_db.csv";

    //private string dataPath = makePath("data", "chirp_cli_db.csv");
    private string dataPath = Path.Combine("chirp_cli_db.csv");

    //why is there two databases?
    static CSVDatabase()
    {
    }

    private CSVDatabase()
    {
        embedded = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        //set the config to "InvariantCulture" and inform the program that the file already has headers
        _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        };
    }

    public static CSVDatabase<T> instance { get; } = new();

    public IEnumerable<T> Read(int? limit = null)
    {
        //ensure file exists

        using var embed = embedded.GetFileInfo("data/chirp_cli_db.csv").CreateReadStream();
        using var reader = new StreamReader(embed);
        using (var csv = new CsvReader(reader, _csvConfig))
        {
            //Makes sure limit is not null, to avoid possible error
            var dataRecords = new List<T>();
            if (limit.HasValue)
                //Gets the newest limited amount of lines in the file
                dataRecords = csv.GetRecords<T>().TakeLast(limit.Value).ToList();
            else
                dataRecords = csv.GetRecords<T>().ToList();

            return dataRecords;
        }
        

    }

    public void Store(T record)
    {
        //create streamwriter and CSVwriter with using
        
        //using (var writer = new StreamWriter(dataPath, true))
        using var embed = embedded.GetFileInfo(dataPath).CreateReadStream();
        using var writer = new StreamWriter(embed);
        using (var csv = new CsvWriter(writer, _csvConfig))
        {
            //add cheep to file then add blank character to end
            if (!File.Exists(dataPath))
            {
                writer.WriteLine("Author,Message,Timestamp");
            }
            csv.WriteRecord(record);
            writer.WriteLine();
        }
    }

    public void setPath(string _datapath)
    {
        dataPath = _datapath;
    }
}
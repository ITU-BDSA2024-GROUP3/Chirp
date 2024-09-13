using System.Collections;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using ArrayList = System.Collections.ArrayList;

namespace SimpleDB;

public record Cheep(string Author, string Message, long Timestamp);


public class CSVDatabase<T> : IDatabaseRepository<T>
{
    //idk much about readonly, rider just said it would be good
    private readonly string _dataPath ="../../data/chirp_cli_db.csv";
    private readonly CsvConfiguration _csvConfig;
    
    private static readonly CSVDatabase<T> Instance = new CSVDatabase<T>();
    
    static CSVDatabase()
    {
    }
    
    private CSVDatabase()
    {
        //set the config to "InvariantCulture" and inform the program that the file already has headers
        _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
    }
    
    public static CSVDatabase<T> instance
    {
        get
        {
            return Instance;
        }
    }

    public IEnumerable<T> Read(int? limit = null)
    {
        //ensure file exists
        if (!File.Exists(_dataPath))
        {
            Console.WriteLine("Data file does not exist.");
            return null;
        }
        
        //create streamreader and CSVreader with "using"
        using (var reader = new StreamReader(_dataPath))
        using (var csv = new CsvReader(reader, _csvConfig))
        {
            //Makes sure limit is not null, to avoid possible error
            var dataRecords = new List<T>();
            if (limit.HasValue)
            {
                //Gets the newest limit amount of lines in the file
                dataRecords = csv.GetRecords<T>().TakeLast(limit.Value).ToList();

            }
            else
            {
                dataRecords = csv.GetRecords<T>().ToList();
            }
            
            return dataRecords;
        }
    }
    
    public void Store(T record)
    {
        //create streamwriter and CSVwriter with using
        using (var writer = new StreamWriter(_dataPath, append: true))
        using (var csv = new CsvWriter(writer, _csvConfig))
        {
            //add cheep to file then add blank character to end
            csv.WriteRecord(record);
            writer.WriteLine();
        }
    }
}

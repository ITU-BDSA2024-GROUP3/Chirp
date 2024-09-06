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
    private readonly string _dataPath;
    private readonly CsvConfiguration _csvConfig;

    public CSVDatabase(string dataPath)
    {
        _dataPath = dataPath;
        //set the config to "InvariantCulture" and inform the program that the file already has headers
        _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };
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
            //assaign a record of cheeps and return them
            var dataRecords = csv.GetRecords<T>().ToList();
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

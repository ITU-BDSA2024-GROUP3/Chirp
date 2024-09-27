﻿using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace SimpleDB;

public class CSVDatabase<T> : IDatabaseRepository<T>
{
    private readonly CsvConfiguration _csvConfig;

    //idk much about readonly, rider just said it would be good
    private string dataPath = "../../data/chirp_cli_db.csv";

   
    static CSVDatabase()
    {
    }

    private CSVDatabase()
    {
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
        if (!File.Exists(dataPath)) throw new FileNotFoundException("Data file not found");

        //create streamreader and CSVreader with "using"
        using (var reader = new StreamReader(dataPath))
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
        using (var writer = new StreamWriter(dataPath, true))
        using (var csv = new CsvWriter(writer, _csvConfig))
        {
            //add cheep to file then add blank character to end
            csv.WriteRecord(record);
            writer.WriteLine();
        }
    }

    public void setPath(string _datapath)
    {
        dataPath = _datapath;
    }
}
﻿using System.Globalization;
using System.Reflection;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Chirp.CSVDBService;

public class CSVDatabase<T> : IDatabaseRepository<T>
{
    private readonly CsvConfiguration _csvConfig;
    
    private string dataPath = Path.Combine("chirp_cli_db.csv");

    //why is there two databases?
    static CSVDatabase()
    {
    }

    private CSVDatabase()
    {
        //set the config to "InvariantCulture" and inform the program that the file already has headers
        _csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {  };
        
        //Checks the data file is there and has a csv file

        

        if (!File.Exists(dataPath))
        {
            File.Create(dataPath);
            
            using (var writer = new StreamWriter(dataPath, true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                //add cheep to file then add blank character to end
                writer.WriteLine("Author,Message,Timestamp");
            }
        }
        }

        public static CSVDatabase<T> instance { get; } = new();

        public IEnumerable<T> Read(int? limit = null) //This does not work
        {
            //ensure file exists

            using (var reader = new StreamReader(dataPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                //Makes sure limit is not null, to avoid possible error
                var dataRecords = new List<T>();
                if (limit.HasValue)
                {
                    //Gets the newest limited amount of lines in the file
                    dataRecords = csv.GetRecords<T>().Take(limit ?? int.MaxValue).ToList();
                }
                else
                {
                    dataRecords = csv.GetRecords<T>().ToList();
                }
                return dataRecords;
            }
        }

        public void Store(T record) //This works
        {
            //create streamwriter and CSVwriter with using
            using (var writer = new StreamWriter(dataPath, true))
            using (var csv = new CsvWriter(writer, _csvConfig))
            {
            csv.WriteRecord(record);
            csv.NextRecord();
        }
    }

    public void setPath(string _datapath)
    {
        dataPath = _datapath;
    }
}
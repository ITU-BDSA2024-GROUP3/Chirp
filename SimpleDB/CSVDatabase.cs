using System.Globalization;
using CsvHelper;

namespace SimpleDB;

public class Cheep
{
    public string Author { get; set; }
    public string Message { get; set; }
    public long Timestamp { get; set; }
}

public class CSVDatabase<T> : IDatabaseRepository<T>
{
    //Reads from the database
    public IEnumerable<T> Read(int? limit = null)
    {
        using (var reader = new StreamReader("./chirp_cli_db.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            IEnumerable<Cheep> records = csv.GetRecords<Cheep>();
            
            return (IEnumerable<T>)records;
        }
    }

    //Stores data in the database
    public void Store(T record)
    {
        
    }
}

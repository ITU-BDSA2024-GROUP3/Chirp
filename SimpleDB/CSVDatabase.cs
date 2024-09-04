namespace SimpleDB;

public class CSVDatabase<T> : IDatabaseRepository<T>
{
    //Reads from the database
    public IEnumerable<T> Read(int? limit = null)
    {
        return null;
    }

    //Stores data in the database
    public void Store(T record)
    {
        
    }
}

using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace SimpleDB;
//namespace System.Data.SQLlite;

public class DBFacade
{
    //reference: https://stackoverflow.com/questions/26020/what-is-the-best-way-to-connect-and-use-a-sqlite-database-from-c-sharp
    SqliteConnection _db;
    //connect to where the database is stored
    public DBFacade(string databasePath) {
        _db = new SqliteConnection(databasePath);
        
    }

    /* void Store(T record)
        {
            //put cheep in database
            //put the data inside the table
        }

    IEnumerable<T> Read(int? limit = null)
    {
        //return cheep from database
        return null;
    }
    */
}
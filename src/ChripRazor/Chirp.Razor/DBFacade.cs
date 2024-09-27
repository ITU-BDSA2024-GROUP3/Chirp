using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace SimpleDB;
//namespace System.Data.SQLlite;

public class DBFacade
{
    //reference: https://stackoverflow.com/questions/26020/what-is-the-best-way-to-connect-and-use-a-sqlite-database-from-c-sharp
    private readonly string _sqlDataPath = "";
    //connect to where the database is stored
    public DBFacade()
    {
        string sqlDataPath = "/tmp/test.db";
        _sqlDataPath = sqlDataPath;
    }

    public List<CheepViewModel> ReadCheeps()
    {
        List<CheepViewModel> cheeps = new List<CheepViewModel>();
        
        var queryString = @"SELECT u.username, m.text, m.pub_date
                                FROM message m
                                JOIN user u ON m.author_id = u.user_id
                                ORDER BY m.pub_date DESC";
        
        return ExecuteQuery(queryString);
    }
}
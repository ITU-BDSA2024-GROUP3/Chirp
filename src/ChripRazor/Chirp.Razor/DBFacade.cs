using Microsoft.Data.Sqlite;
using SQLitePCL;

namespace Chirp.Razor;
//namespace System.Data.SQLlite;

public class DBFacade
{
    //reference: https://stackoverflow.com/questions/26020/what-is-the-best-way-to-connect-and-use-a-sqlite-database-from-c-sharp
    private readonly string _sqlDataPath = "";
    //connect to where the database is stored
    public DBFacade()
    {
        //duplicate string??
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
        
        return Execute(queryString);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        List<CheepViewModel> cheeps = new List<CheepViewModel>();

        
        var sqlQuery = @"SELECT u.username, m.text, m.pub_date
                                FROM message m
                                JOIN user u ON m.author_id = u.user_id
                                WHERE u.username = $author
                                ORDER BY m.pub_date DESC";

        using (var connection = new SqliteConnection($"Data Source={_sqlDataPath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = sqlQuery;
            command.Parameters.AddWithValue("$author", author);
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                
            }
        }

    }

}
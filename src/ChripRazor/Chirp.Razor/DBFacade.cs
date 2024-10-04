using System.Data;
using System.Reflection;
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System.Reflection;
using Chirp.Razor.DomainModel;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.FileProviders;

namespace Chirp.Razor;
//namespace System.Data.SQLlite;

public class DBFacade
{
    //reference: https://stackoverflow.com/questions/26020/what-is-the-best-way-to-connect-and-use-a-sqlite-database-from-c-sharp
    private readonly string _sqlDataPath = "";
    
    private readonly IFileProvider embedded;
    //connect to where the database is stored
    
    string value;
    bool toDelete;
    private readonly ICheepRepository _repository;
    
    public DBFacade(ICheepRepository repository)
    {
        embedded = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
        value = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        _repository = repository;
        if (value == null)
        {
            string path =Path.Combine(Path.GetTempPath(), "chirp.db");
            Environment.SetEnvironmentVariable("CHIRPDBPATH", path);
            toDelete = true;
            
            value = Environment.GetEnvironmentVariable("CHIRPDBPATH");
        }
        _sqlDataPath = value;

        fillDatabase();

    }

    private void fillDatabase()
    {
        using var connect = new SqliteConnection($"Data Source ={_sqlDataPath}");
        
        connect.Open();
        
        var schema = readSqlFile("schema.sql");
        var dump = readSqlFile("dump.sql");
        
        Command(connect, schema);
        Command(connect, dump);
    }
    
    private string readSqlFile(string fileName)
    {
        using var embed = embedded.GetFileInfo(fileName).CreateReadStream();
        using var reader = new StreamReader(embed);
        return reader.ReadToEnd();
    }
    
    private void Command(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    public Task<List<CheepDTO>> ReadCheeps(int page)
    {
        return _repository.ReadCheeps(page, null);
       
    }

    
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int userId, int page)
    {
        return _repository.ReadCheeps(page, userId);
    }
    
    
}
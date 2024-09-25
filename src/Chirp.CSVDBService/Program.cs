using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

CSVDatabase<Cheep> database = CSVDatabase<Cheep>.instance;

app.MapGet("/cheeps", (int? limit) => //What was once read
{
    /*string[] directories = Directory.GetDirectories(Path.Combine("..", "..", "data", "chirp_cli_db.csv"));
    string output = string.Join("\n", directories);
    */
            
    //return new Cheep("error, file not found", Directory.GetCurrentDirectory(), 313131231);
    return database.Read(limit);
    //return new Cheep("Tester", "This is a test", 313131231);
});
app.MapPost("/cheep", (Cheep cheep) => //What was once cheep
{
    database.Store(cheep);
});

app.Run();

public record Cheep(string Author, string Message, long Timestamp);


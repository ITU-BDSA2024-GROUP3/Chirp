using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

CSVDatabase<Cheep> database = CSVDatabase<Cheep>.instance;

app.MapGet("/cheeps", (int? limit) => //What was once read
{
    Console.WriteLine(limit);
    return database.Read(limit);
});
app.MapPost("/cheep", (Cheep cheep) => //What was once cheep
{
    database.Store(cheep);
});

app.Run();

public record Cheep(string Author, string Message, long Timestamp);


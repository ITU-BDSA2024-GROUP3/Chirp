using SimpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

CSVDatabase<Cheep> database = CSVDatabase<Cheep>.instance;

app.MapGet("/cheeps", () => database.Read());
app.MapPost("/cheep", (Cheep cheep) =>
{
    database.Store(cheep);
});

app.Run();

public record Cheep(string Author, string Message, long Timestamp);


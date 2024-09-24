using SimpleDB;
using DocoptNet;

const string usage = @"Chirp CLI version.

Usage:
  chirp 
  chirp data <path>
  chirp (-h | --help)
  chirp --version

Options:
  -h --help     Show this screen.
  --version     Show version.
";

var arguments = new Docopt().Apply(usage, args, version: "0.0.1", exit: true);

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

CSVDatabase<Cheep> database = CSVDatabase<Cheep>.instance;

if ((bool)arguments["data"].Value)
{
    int? limit = null;
    database.setPath(arguments["<path>"].Value.ToString());
    Console.WriteLine($"Use path: {arguments["<path>"].Value.ToString()}");
}
else
{
    Console.WriteLine("Use default path");
}

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


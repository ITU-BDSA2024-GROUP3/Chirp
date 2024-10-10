using Chirp.CSVDBService;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

CSVDatabase<Cheep> database = CSVDatabase<Cheep>.instance;

app.MapGet("/cheeps", (int? limit) => //What was once read
{
    return database.Read(limit);
});
app.MapPost("/cheep", (Cheep cheep) => //What was once cheep
{
    database.Store(cheep);
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.Run();

public record Cheep(string Author, string Message, long Timestamp);


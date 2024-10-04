using Chirp.Razor;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddRazorPages();
//builder.Services.AddSingleton<ICheepService, CheepService>();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();

var app = builder.Build();

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

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ChirpDBContext>();
    context.Database.EnsureCreated();
    DbInitializer.SeedDatabase(context);
}


app.Run();

public partial class Program { }

using ChirpCore;
using ChirpCore.DomainModel;
using ChirpInfrastructure;
using ChirpWeb;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
//builder.Services.AddSingleton<ICheepService, CheepService>();

string? connectionString = builder.Configuration.GetConnectionString("ChirpDBContextConnection");

builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<Author>(options =>
{
    //options.SignIn.RequireConfirmedAccount = true;
    //options.Lockout.AllowedForNewUsers = true;
    options.Password.RequiredLength = 12;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
}).AddEntityFrameworkStores<ChirpDBContext>();

builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<ICheepRepository, CheepRepository>();

builder.Services.AddAuthentication()
    .AddCookie()
    .AddGitHub(o =>
    {
        if (builder.Configuration["GitHub:ClientID"] == null )
        {
            Console.Error.WriteLine("You must provide a client ID.");
        }
        
        if (builder.Configuration["GitHub:ClientSecret"] == null )
        {
            Console.Error.WriteLine("You must provide a client Secret.");
        }
        
        o.ClientId = builder.Configuration["GitHub:ClientID"];
        o.ClientSecret = builder.Configuration["GitHub:ClientSecret"];
        o.CallbackPath = "/signin-github";
    });


    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromSeconds(1800);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
//
// remaining configuration not show
//

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

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

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


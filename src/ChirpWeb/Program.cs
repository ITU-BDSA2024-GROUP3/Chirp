using System.Security.Claims;
using ChirpCore;
using ChirpCore.DomainModel;
using ChirpInfrastructure;
using ChirpWeb;
using Microsoft.AspNetCore.Authentication;
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
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddDistributedMemoryCache();
string? connectionString = builder.Configuration.GetConnectionString("ChirpDBContextConnection");

builder.Services.AddDbContext<ChirpDBContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<Author>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
}).AddEntityFrameworkStores<ChirpDBContext>();


//sets up GitHub authentication 
if (builder.Configuration["GitHubClientSecret"] != null)
{
    builder.Services.AddAuthentication()
        .AddCookie()
        .AddGitHub(o =>
        {
            if (builder.Configuration["GitHubClientID"] == null)
            {
                Console.WriteLine("You must provide a client ID.");
            }
            else
            {
                o.ClientId = builder.Configuration["GitHubClientID"]!;
            }

            if (builder.Configuration["GitHubClientSecret"] == null)
            {
                Console.WriteLine("You must provide a client Secret.");
            }
            else
            {
                o.ClientSecret = builder.Configuration["GitHubClientSecret"]!;
            }

            o.CallbackPath = "/signin-github";

            o.Scope.Add("user:email");

            o.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        });
}


builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromSeconds(1800);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

// remaining configuration not show


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


public partial class Program { }//this is used to get integration tests to work


using System.Data.Common;
using Chirp.Razor.ChirpCore;
using Chirp.Razor.ChirpInfrastucture;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;



namespace integrationtest;

public class TestAPI : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;

    public TestAPI(WebApplicationFactory<Program> fixture)
    {
        _fixture = fixture;
        _fixture.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<ChirpDBContext>));

                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbConnection));

                services.Remove(dbConnectionDescriptor);

                // Create open SqliteConnection so EF won't automatically close it.
                services.AddSingleton<DbConnection>(container =>
                {
                    var connection = new SqliteConnection("DefaultConnection=:memory:");
                    connection.OpenAsync();
                    return connection;
                });

                services.AddDbContext<ChirpDBContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                    
                });
            });
        });
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = true, HandleCookies = true });
        
    }

    [Fact]
    public async void CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public async void CanSeePrivateTimeline(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }

    [Theory]
    [InlineData("What did they take?")]
    public async void CanSeePublicTimelineText(string message)
    {
        var response = await _client.GetAsync($"/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(content);

        var tableElement = htmlDocument.GetElementbyId("messagelist");
        Assert.NotNull(tableElement);
    }
    [Theory]
    [InlineData("What did they take?")]
    public async void CanSeePrivateTimelineText(string message)
    {
        
    }
    /*
     * correctNumberOfCheepsPerPagePublic
     * correctNumberOfCheepsPerPagePrivate
     * correctNumberOfCheepsPerPagePublicNotPage1
     * correctNumberOfCheepsPerPagePrivateNotPage1
     * changePagesChangesTextPublic
     * changePagesChangesTextPrivate
     * TimeStampOnCheepPublic
     * TimeStampOnCheepPrivate
     * AuthorOnCheepPublic
     * AuthorOnCheepPrivate
     * Layout (boxes)?
     */
}
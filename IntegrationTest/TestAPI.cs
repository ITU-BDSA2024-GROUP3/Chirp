using System.Data.Common;
using System.Diagnostics;
using ChirpInfrastructure;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit.Abstractions;


namespace integrationtest;

public class TestAPI : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _fixture;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public TestAPI(WebApplicationFactory<Program> fixture, ITestOutputHelper testOutputHelper)
    {
        _output = testOutputHelper;
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
        _client = _fixture.CreateClient(new WebApplicationFactoryClientOptions
            { AllowAutoRedirect = true, HandleCookies = true });

    }

    private async Task<string> SetPublic()
    {
        var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }

    private async Task<string> SetPublicPage(int page)
    {
        var response = await _client.GetAsync($"/?page={page}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }

    private async Task<string> SetPrivate(string author, int id)
    {
        _output.WriteLine($"Author: {author}, Id: {id}");
        var response = await _client.GetAsync($"/{id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }

    private async Task<string> SetPrivatePage(int page, string author, int id)
    {
        _output.WriteLine($"Author: {author}, Id: {id}");
        var response = await _client.GetAsync($"/{id}/?page={page}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }






    [Fact]
    public async void CanSeePublicTimeline()
    {
        var content = await SetPublic();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }

    [Theory]
    [InlineData("Helge", 11)]
    [InlineData("Adrian", 12)]
    [InlineData("Jacqualine Gilcoine", 10)]
    [InlineData("Roger Histand", 1)]
    [InlineData("Quintin Sitts", 5)]
    [InlineData("Wendell Ballan", 3)]
    public async void CanSeePrivateTimeline(string author, int id)
    {
        var content = await SetPrivate(author, id);

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);

    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(3)]
    [InlineData(20)]
    [InlineData(16)]
    [InlineData(7)]

    public async void CanSeeCheepsPublic(int page)
    {
        var content = await SetPublicPage(1);
        Assert.DoesNotContain("There are no cheeps so far.", content);
    }

    [Theory]
    [InlineData("Helge", 11)]
    [InlineData("Adrian", 12)]
    [InlineData("Jacqualine Gilcoine", 10)]
    [InlineData("Roger Histand", 1)]
    [InlineData("Quintin Sitts", 5)]
    [InlineData("Wendell Ballan", 3)]
    public async void CanSeeCheepsPrivate(string author, int id)
    {
        var content = await SetPrivate(author, id);
        Assert.DoesNotContain("There are no cheeps so far.", content);

    }

    [Theory]
    [InlineData(
        "They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me.")]
    [InlineData("At last we came back!")]

    public async void CanSeePublicTimelineText(string message)
    {
        var content = await SetPublic();
        Assert.Contains(message, content);

    }

    [Theory]
    [InlineData("The shores of the middle of it, and you can imagine, it was probable, from the hall.", 2)]
    [InlineData("I left the room.", 12)]
    public async void CanSeePublicTimelineTestDifferentPages(string message, int page)
    {
        var content = await SetPublicPage(page);
        Assert.Contains(message, content);
    }



    [Theory]
    [InlineData("Hello, BDSA students!", "Helge", 11)] //Change to ids to find page
    [InlineData("Hej, velkommen til kurset.", "Adrian", 12)] //change messages
    public async void CanSeePrivateTimelineText(string message, string author, int id)
    {
        var content = await SetPrivate(author, id);
        Assert.Contains(message, content);
    }

    [Theory]
    [InlineData("At the same height.", "Jacqualine Gilcoine", 10, 2)]
    public async void CanSeePrivateTimelineTestDifferentPages(string message, string author, int id, int page)
    {
        var content = await SetPrivatePage(page, author, id);
        Assert.Contains(message, content);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(16)]


    public async void CorrectNumberOfCheepsPerPagePublic(int page)
    {
        var content = await SetPublicPage(page);
        int first = content.Length;
        string newString = content.Replace("<li>", "");
        Assert.Equal(32, (first - newString.Length) / 4); //takes all cheeps
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", 10, 1)]
    [InlineData("Jacqualine Gilcoine", 10, 2)]
    [InlineData("Jacqualine Gilcoine", 10, 3)]
    [InlineData("Roger Histand", 1, 1)]
    [InlineData("Quintin Sitts", 5, 2)]



    public async void CorrectNumberOfCheepsPerPagePrivate(string author, int id, int page)
    {
        var content = await SetPrivatePage(page, author, id);
        int first = content.Length;
        string newString = content.Replace("<li>", "");
        Assert.Equal(32, (first - newString.Length) / 4); //takes all cheeps
    }

    [Fact]
    public async void HomepageIsSameAsFirstPagePublic()
    {
        var content = await SetPublic();
        var content2 = await SetPublicPage(1);
        Assert.Equal(content, content2);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", 10)]
    [InlineData("Helge", 11)]
    [InlineData("Adrian", 12)]
    [InlineData("Roger Histand", 1)]
    [InlineData("Quintin Sitts", 5)]
    [InlineData("Wendell Ballan", 3)]
    public async void HomepageIsSameFirstPagePrivate(string author, int id)
    {
        var content = await SetPrivate(author, id);
        var content2 = await SetPrivatePage(1, author, id);
        Assert.Equal(content, content2);
    }

    [Theory]
    [InlineData("Roger Histand", 1, 2)]
    [InlineData("Adrian", 12, 1)]
    [InlineData("Helge", 11, 1)]
    public async void SometimesThereAreLessThan32CheepsPrivate(string author, int id, int page)
    {
        var content = await SetPrivatePage(page, author, id);
        int first = content.Length;
        string newString = content.Replace("<li>", "");
        Assert.True(32 > (first - newString.Length) / 4);
    }

    [Fact]
    public async void SometimesThereAreLessThan32CheepsPublic()
    {
        var content = await SetPublicPage(21);
        int first = content.Length;
        string newString = content.Replace("<li>", "");
        Assert.True(32 > (first - newString.Length) / 4);
    }

    [Theory]

    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(45)]



    public async void PagesCanChangePublic(int page)
    {
        var content = await SetPublicPage(page);
        Assert.Contains($"?page={page - 1}", content);
        Assert.Contains($"?page={page + 1}", content);

    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", 10, 2)]
    [InlineData("Jacqualine Gilcoine", 10, 3)]
    [InlineData("Jacqualine Gilcoine", 10, 45)]


    public async void PagesCanChangePrivate(string author, int id, int page)
    {
        var content = await SetPrivatePage(page, author, id);
        Assert.Contains($"?page={page - 1}", content);
        Assert.Contains($"?page={page + 1}", content);

    }

    [Fact]

    public async void ButtonsExistMainPublic()
    {
        var content = await SetPublic();
        Assert.Contains("Button", content);
        Assert.Contains("btn", content);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", 10)]

    public async void ButtonsExistMainPrivate(string author, int id)
    {
        var content = await SetPrivate(author, id);
        Assert.Contains("Button", content);
        Assert.Contains("btn", content);

    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]


    public async void ButtonsExistPublic(int page)
    {
        var content = await SetPublicPage(page);
        Assert.Contains("Button", content);
        Assert.Contains("btn", content);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", 10, 2)]
    [InlineData("Jacqualine Gilcoine", 10, 3)]
    [InlineData("Jacqualine Gilcoine", 10, 4)]
    public async void ButtonsExistPrivate(string author, int id, int page)
    {
        var content = await SetPrivatePage(page, author, id);
        Assert.Contains("Button", content);
        Assert.Contains("btn", content);

    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(10)]

    public async void ButtonLinksCorrectlyPublic(int page)
    {
        var content = await SetPublicPage(page);
        bool windows1 = content.Contains(
            $"<button class=\"btn\">\r\n        <a href=\"?page={page + 1}\" class=\"btn\" id=\"nextBtn\" style=\"color: white;\">Next ({page + 1})</a>\r\n    </button>\r"
        );
        bool windows2 = content.Contains(
                $"<Button class=\"btn\">\r\n        <a href=\"?page={page - 1}\" class=\"btn\" id=\"prevBtn\" style=\"color: white;\">Previous ({page - 1})</a>\r\n    </Button>\r");
        bool linux1 = content.Contains(
            $"<button class=\"btn\">\n        <a href=\"?page={page + 1}\" class=\"btn\" id=\"nextBtn\" style=\"color: white;\">Next ({page + 1})</a>\n    </button>"
        );
        bool linux2 = content.Contains(
            $"<Button class=\"btn\">\n        <a href=\"?page={page - 1}\" class=\"btn\" id=\"prevBtn\" style=\"color: white;\">Previous ({page - 1})</a>\n    </Button>");

        
        
        Assert.True((windows1 && windows2)||(linux1 && linux2));
    }

    [Theory]
    [InlineData("Adrian", 12, 2)]
    [InlineData("Jacqualine Gilcoine", 10, 2)]
    public async void ButtonLinksCorrectlyPrivate(string author, int id, int page)
    {
        var content = await SetPrivatePage(page, author, id);
        bool windows1 = content.Contains(    
            $"<button class=\"btn\">\r\n        <a href=\"/{id}?page={page + 1}\" class=\"btn\" id=\"nextBtn\" style=\"color: white;\">Next ({page + 1})</a>\r\n    </button>\r");
        bool linux1 = content.Contains( 
            $"<button class=\"btn\">\n        <a href=\"/{id}?page={page + 1}\" class=\"btn\" id=\"nextBtn\" style=\"color: white;\">Next ({page + 1})</a>\n    </button>");
        bool windows2 =
            content.Contains(
                $"<Button class=\"btn\">\r\n        <a href=\"/{id}?page={page - 1}\" class=\"btn\" id=\"prevBtn\" style=\"color: white;\">Previous ({page - 1})</a>\r\n    </Button>\r");
        bool linux2 =
            content.Contains(
                $"<Button class=\"btn\">\n        <a href=\"/{id}?page={page - 1}\" class=\"btn\" id=\"prevBtn\" style=\"color: white;\">Previous ({page - 1})</a>\n    </Button>");
        
        Assert.True((windows1 && windows2) || (linux2 && linux1));
        
        
        
        /*Assert.Contains(
            $"<button class=\"btn\">\r\n        <a href=\"/{id}?page={page + 1}\" class=\"btn\" id=\"nextBtn\" style=\"color: white;\">Next ({page + 1})</a>\r\n    </button>\r",
            content);
        Assert.Contains(
            $"<Button class=\"btn\">\r\n        <a href=\"/{id}?page={page - 1}\" class=\"btn\" id=\"prevBtn\" style=\"color: white;\">Previous ({page - 1})</a>\r\n    </Button>\r",
            content);*/
    }

    [Fact]
    public async void ThereAreNoMoreCheepsPublic()
    {
        var content = await SetPublicPage(22);
        Assert.Contains("There are no cheeps so far.", content);
        Assert.DoesNotContain("<li>", content);
    }

    [Fact]
    public async void ThereAreNoMoreCheepsPrivate()
    {
        var content = await SetPrivatePage(69, "Jacqualine Gilcoine", 10);
        Assert.Contains("There are no cheeps so far.", content);
        Assert.DoesNotContain("<li>", content);
    }

    [Theory]
    [InlineData(1, 2,
        "They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me.")]
    [InlineData(1, 2, "At last we came back!")]
    [InlineData(21, 22, "Hej, velkommen til kurset.")]
    [InlineData(20, 21, "The one is very hard, and yesterday evening in an open door leading to the staple fuel.")]

    public async void CheepsChangeWhenPagesChangePublic(int page1, int page2, string message)//duplicate cheeps?
    {
        var content1 = await SetPublicPage(page1);
        var content2 = await SetPublicPage(page2);
        Assert.Contains(message, content1);
        Assert.DoesNotContain(message, content2);
    }

    [Theory]
    [InlineData(1, 2,
        "They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me.",
        "Jacqualine Gilcoine", 10)]

    public async void CheepChangeWhenPagesChangePrivate(int page1, int page2, string message, string author, int id)
    {
        var content1 = await SetPrivatePage(page1, author, id);
        var content2 = await SetPrivatePage(page2, author, id);
        Assert.Contains(message, content1);
        Assert.DoesNotContain(message, content2);
    }

    [Theory]
    [InlineData("08/01/23 11.14.58")]
    [InlineData("08/01/23 11.14.11")]
    public async void TimeStampsExistsPublic(string timeStamp)
    {
        var content = await SetPublic();
        Assert.Contains(timeStamp, content);
    }

    [Theory]
    [InlineData("08/01/23 11.15.21", "Jacqualine Gilcoine", 10)]
    public async void TimeStampsExistsPrivate(string timeStamp, string author, int id)
    {
        var content = await SetPrivate(author, id);
        Assert.Contains(timeStamp, content);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", 10,
        "They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me.",
        "08/01/23 11.14.37", 1)]
    public async void ElementsOfCheepsAreCorrectPublic(string author, int id, string message, string timestamp,
        int page)
    {
        var content = await SetPublicPage(1);
        bool windows = content.Contains(
            $"<li>\r\n                    <p>\r\n                        <strong>\r\n                            <a href=\"/{id}/?page=1\">{author}</a>\r\n                        </strong>\r\n                        {message}\r\n                        <small>&mdash; {timestamp}</small>\r\n                    </p>\r\n                </li>\r\n");
        bool linux = content.Contains(
            $"<li>\n                    <p>\n                        <strong>\n                            <a href=\"/{id}/?page=1\">{author}</a>\n                        </strong>\n                        {message}\n                        <small>&mdash; {timestamp}</small>\n                    </p>\n                </li>\n");
        Assert.True(windows || linux);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", 10,
        "They were married in Chicago, with old Smith, and was expected aboard every day; meantime, the two went past me.",
        "08/01/23 11.14.37", 1)]
    public async void ElementsOfCheepsAreCorrectPrivate(string author, int id, string message, string timestamp,
        int page)
    {
        var content = await SetPrivatePage(page, author, id);
        bool windows= content.Contains(
            $"<li>\r\n                    <p>\r\n                        <strong>\r\n                            <a href=\"/{id}\">{author}</a>\r\n                        </strong>\r\n                        {message}\r\n                        <small>&mdash; {timestamp}</small>\r\n                    </p>\r\n                </li>\r\n");
        bool linux= content.Contains(
            $"<li>\n                    <p>\n                        <strong>\n                            <a href=\"/{id}\">{author}</a>\n                        </strong>\n                        {message}\n                        <small>&mdash; {timestamp}</small>\n                    </p>\n                </li>\n");
        Assert.True(windows || linux);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(10)]
    [InlineData(20)]

    public async void TimeStampAlwaysExistsPublic(int page)
    {
        var content = await SetPublicPage(page);
        Assert.DoesNotContain($"<small>&mdash;</small>\r\n", content);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", 10, 1)]

    public async void TimeStampAlwaysExistsPrivate(string author, int id, int page)
    {
        var content = await SetPrivatePage(page, author, id);
        Assert.DoesNotContain($"<small>&mdash;</small>\r\n", content);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]

    public async void AuthorAlwaysExistsPublic(int page)
    {
        var content = await SetPublicPage(page);
        Assert.DoesNotContain("<a href=\"//?page=1\"></a>", content);
    }
    [InlineData("Jacqualine Gilcoine", 10, 1)]
    [InlineData("Jacqualine Gilcoine", 10, 2)]
    public async void AuthorAlwaysExistsPrivate(string author, int id, int page)
    {
        var content = await SetPrivatePage(page, author, id);
        Assert.DoesNotContain($"<a href=\"/\"></a>", content);
    }

    [Theory]
    [InlineData("Adrian", 12, 21)]
    [InlineData("Helge", 11, 21)]
    [InlineData("Jacqualine Gilcoine", 10, 1)]
    public async void AuthorsExistPublic(string author, int id, int page)
    {
        var content = await SetPublicPage(page);
        Assert.Contains(author, content);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", 10, 1)]
    [InlineData("Jacqualine Gilcoine", 10, 2)]

    public async void PrivateTimelineOnlyHasOneAuthor(string author, int id, int page)
    {
        var content = await SetPrivatePage(page, author, id);
        int first = content.Length;
        string replace = $"<a href=\"/{id}\">";
        string content2 = content.Replace(replace, "");
        
        if (id > 9)
        {
            Assert.Equal(32*(18-4), first-content2.Length);
        }
        else
        {
            Assert.Equal(32*(18-5), first-content2.Length);

        }
        
        

    }
    [Theory]
    [InlineData("Adrian", 12, 21)]
    public async void AuthorLinksExistPublic(string author, int id, int page)
    {
        var content = await SetPublicPage(page);
        Assert.Contains($"<a href=\"/{id}/?page=1\">{author}</a>", content);//should this be kept?
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", 10, 2)]
    [InlineData("Adrian", 12, 1)]

    public async void AuthorLinksExistPrivate(string author, int id, int page)
    {
        var content = await SetPrivatePage(page, author, id);
        Assert.Contains($"<a href=\"/{id}\">{author}</a>", content);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]

    public async void CorrectStyleSheetUsedPublic(int page)
    {
        var content = await SetPublicPage(page);
        Assert.Contains("<link href=\"/css/style.css\" rel=\"stylesheet\" type=\"text/css\" />", content);
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine", 10, 1)]
    [InlineData("Jacqualine Gilcoine", 10, 2)]
    [InlineData("Adrian", 12, 1)]
    [InlineData("Helge", 11, 1)]


    public async void CorrectStyleSheetUsedPrivate(string author, int id, int page)
    {
        var content = await SetPrivatePage(page, author, id);
        Assert.Contains("<link href=\"/css/style.css\" rel=\"stylesheet\" type=\"text/css\" />", content);
    }
    
    /*
    
    
     * NoPreviousPage1OnHomePage
     * PressingButtonWorksPublic
     * PressingButtonWorksPrivate
     * TextAlwaysExists
     */
    
    
    
    
    /*[Fact]
    //[InlineData("What did they take?")]
    public async void CanSeePublicTimelineCheep()
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
    [InlineData("Helge", 11)] //Change to ids to find page
    [InlineData("Adrian", 12)]
    public async void CanSeePrivateTimelineCheep(string author, int id)
    {
        _output.WriteLine($"Author: {author}, Id: {id}");
        var response = await _client.GetAsync($"/{id}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(content);

        var tableElement = htmlDocument.GetElementbyId("messagelist");
        Assert.NotNull(tableElement);
    }*/
}
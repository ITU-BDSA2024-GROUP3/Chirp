using Microsoft.AspNetCore.Mvc.Testing;
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
    [InlineData("Helge", 11)] //Change to ids to find page
    [InlineData("Adrian", 12)]
    public async void CanSeePrivateTimeline(string author,int id)
    {
        _output.WriteLine($"Author: {author}, Id: {id}");
        var response = await _client.GetAsync($"/{id}");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
        
    }
}
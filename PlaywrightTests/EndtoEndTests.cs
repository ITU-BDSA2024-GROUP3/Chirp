using Microsoft.Playwright;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using PlaywrightTests;

[TestFixture]
public class EndToEndTests: PageTest{
    private Process _serverProcess;
    [SetUp]
    public async Task Setup()
    {
       //run process that enables "dotnet run" of the project
       _serverProcess = await EndToEndTestsUtility.StartServer();
    }

    [TearDown]
    public async Task TearDown()
    {
        //ensures that every process is closed again
        _serverProcess.Kill();
        _serverProcess.Dispose();
    }

    [Test]
    public async Task HomePageTest()
    {
        await Page.GotoAsync("http://localhost:5273/");
       
        await Expect(Page
                .GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" }))
            .ToBeVisibleAsync();
        await Expect(Page).ToHaveURLAsync(new Regex("http://localhost:5273/"));
        await Expect(Page).ToHaveTitleAsync(new Regex("Chirp!"));
    }

    [Test]
    public async Task UserTimelineTest()
    {
        //got to Jacqulaine's timeline
        await Page.GotoAsync("http://localhost:5273/");
        await Page.Locator("p").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Link)
            .ClickAsync();
        
        await Expect(Page
                .GetByRole(AriaRole.Heading, new() { Name = "Jacqualine Gilcoine's Timeline" }))
            .ToBeVisibleAsync();
        await Expect(Page).ToHaveURLAsync("http://localhost:5273/Jacqualine%20Gilcoine?page=1");
        
        //can return to public timeline
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex("http://localhost:5273/"));
    }

    [Test]
    public async Task registerPageTest()
    {
        await Page.GotoAsync("http://localhost:5273/");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        //await Page.GetByRole(AriaRole.Heading, new() { Name = "Register", Exact = true }).ClickAsync();
        //await Page.GetByRole(AriaRole.Heading, new() { Name = "Create a new account." }).ClickAsync();
        await Page.GetByText("Username").ClickAsync();
        await Page.GetByText("Email").ClickAsync();
        await Page.GetByText("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByText("Confirm Password").ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Confirm Password").ClickAsync();
        
        await Expect(Page).ToHaveTitleAsync(new Regex("Register"));
        
        
    }

    [Test]
    public async Task loginPageTest()
    {
        await Page.GotoAsync("http://localhost:5273/");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Log in", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Use a local account to log in." }).ClickAsync();
        await Page.GetByPlaceholder("name").ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByText("Remember me?").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync(new Regex("Log in"));
    }
}

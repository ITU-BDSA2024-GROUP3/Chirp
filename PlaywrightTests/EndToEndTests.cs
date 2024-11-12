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
    public async Task RegisterPageTest()
    { 
        await Page.GotoAsync("http://localhost:5273/");
        
        //go to register page
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
       
        await Expect(Page).ToHaveTitleAsync(new Regex("Register"));
        await Expect(Page).ToHaveURLAsync("http://localhost:5273/Identity/Account/Register");
        
        await Expect(Page.GetByText("Username")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Email")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Password", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Confirm Password")).ToBeVisibleAsync();
        
        //Click register button
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        
        await Expect(Page.GetByText("Username The UserName field is required.")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Email The Email field is required.")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Password The Password field is required.")).ToBeVisibleAsync();
    }

    [Test]
    public async Task LoginPageTest()
    {
        await Page.GotoAsync("http://localhost:5273/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        
        await Expect(Page).ToHaveTitleAsync(new Regex("Log in"));
        await Expect(Page).ToHaveURLAsync("http://localhost:5273/Identity/Account/Login");
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Log in", Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Use a local account to log in." })).ToBeVisibleAsync();
        await Expect(Page.GetByPlaceholder("name")).ToBeVisibleAsync();
        await Expect(Page.GetByPlaceholder("password")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Remember me?")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Log in" })).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register as a new user" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync(new Regex("Register"));
        await Expect(Page).ToHaveURLAsync("http://localhost:5273/Identity/Account/Register?returnUrl=%2F");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        
    }
}

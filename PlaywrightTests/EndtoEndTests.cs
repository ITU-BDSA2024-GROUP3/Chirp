using Microsoft.Playwright;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using PlaywrightTests;

[TestFixture]
public class EndToEndTests: PageTest{
    private Process _serverProcess;
    [SetUp]
    public async Task Setup()
    {
       //does not work properluy
       //_serverProcess = await EndToEndTestsUtility.StartServer();
       
       
    }

    [TearDown]
    public async Task TearDown()
    {
        _serverProcess.Kill();
        _serverProcess.Dispose();
    }

    [Test]
    public async Task UITest()
    {
        
        _serverProcess = new Process();

        ProcessStartInfo startInfo = new ProcessStartInfo()
        {
            FileName = "dotnet",
            Arguments = "run", //--project ./../../../../../src/ChirpWeb",
            WorkingDirectory = "./../../../../src/ChirpWeb",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        //_serverProcess.StartInfo.WorkingDirectory = "C:\\Users\\Ninja\\Repos\\Uni\\Semester 3\\Software Architecture\\Chirp\\src\\ChirpWeb";

        _serverProcess.StartInfo = startInfo;
        _serverProcess.Start();
        
        await Task.Delay(10000);
        
        await Page.GotoAsync("http://localhost:5273/");
        
        /*
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Icon1Chirp!" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" }).ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).ClickAsync();
        await Page.GetByText("Jacqualine Gilcoine Starbuck").ClickAsync();
        await Page.Locator("p").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Link)
            .ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "Mellie Yost But what was" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Mellie Yost" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Register", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Create a new account." }).ClickAsync();
        await Page.GetByText("Username").ClickAsync();
        await Page.GetByText("Email").ClickAsync();
        await Page.GetByText("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByText("Confirm Password").ClickAsync();
        await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await Page.GetByPlaceholder("name@example.com").ClickAsync();
        await Page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
        await Page.GetByLabel("Confirm Password").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
        await Page.GetByText("Create a new account. Username The UserName field is required. Email The Email")
            .ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Log in", Exact = true }).ClickAsync();
        await Page.GetByRole(AriaRole.Heading, new() { Name = "Use a local account to log in." }).ClickAsync();
        await Page.GetByPlaceholder("name").ClickAsync();
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByText("Remember me?").ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        */
    }

    [Test]
    public async Task gobacktofrontpage()
    {
        /*
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        await Page.GetByText("Jacqualine Gilcoine Starbuck").ClickAsync();
        await Page.GetByText("— 08/01/23 11.17.39").ClickAsync();
        */
    }
}

using Microsoft.Playwright;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright.NUnit;
using EndToEndTest;

[TestFixture]
public class EndToEndTests: PageTest{
    private Process _serverProcess;
    [SetUp]
    public async Task Setup()
    {
       //run process that enables "dotnet run" of the project
       _serverProcess = await EndToEndTestsUtility.StartServer();
       await Page.GotoAsync("http://localhost:5273/");
       
       //create hans
       
       //logout
       //EndToEndTestsUtility.UserLogOut(Page, "hans");
    }

    [TearDown]
    public async Task TearDown()
    {
        
        //kill hans
        //EndToEndTestsUtility.UserDelete(Page, "hans@grethe.com", "Abc123456789");
        //ensures that every process is closed again
        _serverProcess.Kill();
        _serverProcess.Dispose();
    }
    
    [Test]
    public async Task HomePageTest()
    {
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
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Link).ClickAsync();
        
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
    /*
    [Test]
    public async Task LogInUserTest()
    {
        await EndToEndTestsUtility.UserLogIn(Page, "hans@grethe.com", "Abc123456789");
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();

        await Page.Locator("#Text").DblClickAsync();
        await Page.Locator("#Text").FillAsync("What's on your mind hans?");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "What's on your mind hans?" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#Text")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Expect(Page.GetByText("hans's Timeline What's on")).ToBeVisibleAsync();
        await Expect(Page.GetByText("hans I exist Likes: 0")).ToBeVisibleAsync();

        await EndToEndTestsUtility.UserLogOut(Page, "hans");
        await Expect(Page).ToHaveURLAsync(new Regex("http://localhost:5273/"));
        
        EndToEndTestsUtility.UserDelete(Page, "hans@grethe.com", "Abc123456789");

    }

    [Test]
    public async Task followTest()
    {
        await EndToEndTestsUtility.UserLogIn(Page, "hans@grethe.com", "Abc123456789");
        
        //go to public timeline after loging in
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        
        //follow Jacqualine
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. Likes: 0 ♡" }).GetByRole(AriaRole.Button).First.ClickAsync();
        
        await Page.Locator("#Text").DblClickAsync();
        await Page.Locator("#Text").FillAsync("I exist");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        //go to my timeline
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        
        //ensure own and Jacqualine Gilcone cheeps is displayed on private timeline
        await Expect(Page.GetByText("hans I exist Likes: 0")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Jacqualine Gilcoine Unfollow Starbuck now is what we hear the worst. Likes: 0")).ToBeVisibleAsync();
        
        //unfollow Jacqualine
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Unfollow Starbuck now is what we hear the worst. Likes: 0" }).GetByRole(AriaRole.Button).First.ClickAsync();
        
        //ensure own cheeps are displayed
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Next (2)" }).ClickAsync();
       
        await Expect(Page.GetByText("There are no cheeps so far.")).ToBeVisibleAsync();

        EndToEndTestsUtility.UserDelete(Page, "hans@grethe.com", "Abc123456789");

        //await EndToEndTestsUtility.UserLogOut(Page, "hans");
    }

    [Test]
    public async Task likeTest()
    {
        await EndToEndTestsUtility.UserLogIn(Page, "hans@grethe.com", "Abc123456789");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();

        await Page.Locator("li")
            .Filter(new() { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. Likes: 0 ♡" })
            .GetByRole(AriaRole.Button).Nth(1).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();

        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 1");
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "♥︎" }).ClickAsync();
        
        
        await Expect(Page.Locator("#messagelist")).ToContainTextAsync("Likes: 0");
        
        EndToEndTestsUtility.UserDelete(Page, "hans@grethe.com", "Abc123456789");

        //await EndToEndTestsUtility.UserLogOut(Page, "hans");

    }
    
    
    [Test]
    public async Task personalDataPageTest()
    {
        await EndToEndTestsUtility.UserLogIn(Page, "hans@grethe.com", "Abc123456789");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Manage hans's account" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "See your account information" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Username: hans" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Email: hans@grethe.com" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Listitem)).ToBeVisibleAsync();
        
        EndToEndTestsUtility.UserDelete(Page, "hans@grethe.com", "Abc123456789");
        //await EndToEndTestsUtility.UserLogOut(Page, "hans");


    }

    [Test]
    public async Task personalDataPageDownloadTest()
    {
        await EndToEndTestsUtility.UserLogIn(Page, "hans@grethe.com", "Abc123456789");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "Manage hans's account" }).ClickAsync();
        
        var download = await Page.RunAndWaitForDownloadAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Download" }).ClickAsync();
        });

        EndToEndTestsUtility.UserDelete(Page, "hans@grethe.com", "Abc123456789");

        //await EndToEndTestsUtility.UserLogOut(Page, "hans");


    }

    [Test]
    public async Task personalDataPageDeleteTest()
    {
        await EndToEndTestsUtility.UserLogIn(Page, "hans@grethe.com", "Abc123456789");
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();

        await Page.Locator("#Text").DblClickAsync();
        await Page.Locator("#Text").FillAsync("I have posted a cheep");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. Likes: 0 ♡" }).GetByRole(AriaRole.Button).First.ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Manage hans's account" }).ClickAsync();
        await Expect(Page.Locator("p").Filter(new() { HasText = "Jacqualine Gilcoine" })).ToBeVisibleAsync();
        await Expect(Page.Locator("li").Filter(new() { HasText = "hans I have posted a cheep" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Forget me!" }).ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await Page.GetByPlaceholder("Please enter your password.").FillAsync("Abc123456789");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "hans" })).ToBeHiddenAsync();
        
    }
    
    */
    [Test]
    public async Task loginTestHelge()
    {
        await EndToEndTestsUtility.UserLogIn(Page, "ropf@itu.dk", "LetM31n!");
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "What's on your mind Helge?" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).ToBeVisibleAsync();
        
        EndToEndTestsUtility.UserLogOut(Page, "ropf@itu.dk");
        await Expect(Page).ToHaveURLAsync(new Regex("http://localhost:5273/"));
    }
    [Test]
    public async Task followingtest2()
    {
        //register user
        EndToEndTestsUtility.UserRegister(Page, "dorthe@mail.com","dorthe", "Abc123456789");
        
        //follow testing
        await Expect(Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. Likes: 1" }).GetByRole(AriaRole.Button).First).ToBeVisibleAsync();
        await Expect(Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. Likes: 1" }).GetByRole(AriaRole.Paragraph).Nth(1)).ToBeVisibleAsync();
        
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. Likes: 1" }).GetByRole(AriaRole.Button).First.ClickAsync();
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Unfollow Starbuck now is what we hear the worst. Likes: 1" }).GetByRole(AriaRole.Button).First).ToBeVisibleAsync();

        EndToEndTestsUtility.UserDelete(Page, "dorthe@mail.com", "Abc123456789");
    }

    [Test]
    public async Task liketest2()
    {
        EndToEndTestsUtility.UserRegister(Page, "lars@mail.com","lars", "Abc123456789");
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. Likes: 1 ♡" }).GetByRole(AriaRole.Button).Nth(1).ClickAsync();
       
        await Expect(Page.GetByText("Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. Likes: 2 ♥︎")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "♥︎" }).ClickAsync();
        await Expect(Page.GetByText("Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. Likes: 1 ♡")).ToBeVisibleAsync();

        EndToEndTestsUtility.UserDelete(Page, "lars@mail.com", "Abc123456789");

    }
    
    [Test]
    public async Task megaTest()
    {
        //register user
        EndToEndTestsUtility.UserRegister(Page, "hans@grethe.com","hans", "Abc123456789");
        
        //follow testing
        await Expect(Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. Likes: 0 ♡" }).GetByRole(AriaRole.Button).First).ToBeVisibleAsync();
        await Expect(Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. Likes: 0 ♡" }).GetByRole(AriaRole.Paragraph).Nth(1)).ToBeVisibleAsync();
        
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Follow Starbuck now is what we hear the worst. Likes: 0 ♡" }).GetByRole(AriaRole.Button).First.ClickAsync();
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Unfollow Starbuck now is what we hear the worst. Likes: 0" }).GetByRole(AriaRole.Button).First).ToBeVisibleAsync();
        
        //like testing
        await Page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Unfollow Starbuck now is what we hear the worst. Likes: 0" }).GetByRole(AriaRole.Button).Nth(1).ClickAsync();
        
        await Expect(Page.GetByText("Likes: 1")).ToBeVisibleAsync();
        
        await Page.Locator("#Text").ClickAsync();
        await Page.Locator("#Text").FillAsync("THIS IS THE LAST TIME ALL IN ONE TEST");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();
        
        await Expect(Page.GetByText("hans THIS IS THE LAST TIME")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        
        await Expect(Page.GetByText("hans THIS IS THE LAST TIME")).ToBeVisibleAsync();
        
        await Page.GetByText("Jacqualine Gilcoine Unfollow Starbuck now is what we hear the worst. Likes: 1").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Manage hans's account" }).ClickAsync();
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "See your account information" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Jacqualine Gilcoine" })).ToBeVisibleAsync();
        await Expect(Page.Locator("li").Filter(new() { HasText = "hans THIS IS THE LAST TIME" })).ToBeVisibleAsync();
        
        var download = await Page.RunAndWaitForDownloadAsync(async () =>
        {
            await Page.GetByRole(AriaRole.Button, new() { Name = "Download" }).ClickAsync();
        });
        
        EndToEndTestsUtility.UserDelete(Page, "hans@grethe.com", "Abc123456789");
        
    }
    
    [Test]
    public async Task megaTest2()
    {
        EndToEndTestsUtility.UserRegister(Page, "bob@bob.com","bob", "Abc123456789");
        
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "Mellie Yost But what was" })).ToBeVisibleAsync();
        await Expect(Page.Locator("li").Filter(new() { HasText = "Mellie Yost But what was" }).GetByRole(AriaRole.Paragraph).Nth(1)).ToBeVisibleAsync();
        
        await Page.Locator("li").Filter(new() { HasText = "Mellie Yost But what was" }).GetByRole(AriaRole.Button).First.ClickAsync();
        
        await Expect(Page.Locator("li").Filter(new() { HasText = "Mellie Yost But what was" }).GetByRole(AriaRole.Button).First).ToBeVisibleAsync();
        
        await Page.Locator("li").Filter(new() { HasText = "Mellie Yost But what was" }).GetByRole(AriaRole.Button).Nth(1).ClickAsync();
        
        await Expect(Page.GetByText("Likes: 1").Nth(1)).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "my timeline" }).ClickAsync();
        
        await Page.GetByText("Mellie Yost But what was").ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Manage hans's account" }).ClickAsync();
        
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "See your account information" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Mellie Yost " })).ToBeVisibleAsync();
        
        
        EndToEndTestsUtility.UserDelete(Page, "bob@bob.com", "Abc123456789");
        
    }
    
    /*
await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
await Page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
await Page.GetByPlaceholder("name", new() { Exact = true }).FillAsync("hans");
await Page.GetByPlaceholder("name", new() { Exact = true }).PressAsync("Tab");
await Page.GetByPlaceholder("name@example.com").FillAsync("hans@grethe.com");
await Page.GetByPlaceholder("name@example.com").PressAsync("Tab");
await Page.GetByLabel("Password", new() { Exact = true }).FillAsync("Abc123456789");
await Page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");
await Page.GetByLabel("Confirm Password").FillAsync("Abc123456789");
await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();*/
    
}

using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
{
    Headless = false,
});
var context = await browser.NewContextAsync();

var page = await context.NewPageAsync();
await page.GotoAsync("http://localhost:5273/");
await page.GetByRole(AriaRole.Heading, new() { Name = "Icon1Chirp!" }).ClickAsync();
await page.GetByRole(AriaRole.Heading, new() { Name = "Public Timeline" }).ClickAsync();
await page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).ClickAsync();
await page.GetByText("Jacqualine Gilcoine Starbuck").ClickAsync();
await page.Locator("p").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).GetByRole(AriaRole.Link).ClickAsync();
await page.Locator("li").Filter(new() { HasText = "Jacqualine Gilcoine Starbuck" }).ClickAsync();
await page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
await page.Locator("li").Filter(new() { HasText = "Mellie Yost But what was" }).ClickAsync();
await page.GetByRole(AriaRole.Link, new() { Name = "Mellie Yost" }).ClickAsync();
await page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
await page.GetByRole(AriaRole.Heading, new() { Name = "Register", Exact = true }).ClickAsync();
await page.GetByRole(AriaRole.Heading, new() { Name = "Create a new account." }).ClickAsync();
await page.GetByText("Username").ClickAsync();
await page.GetByText("Email").ClickAsync();
await page.GetByText("Password", new() { Exact = true }).ClickAsync();
await page.GetByText("Confirm Password").ClickAsync();
await page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
await page.GetByPlaceholder("name@example.com").ClickAsync();
await page.GetByLabel("Password", new() { Exact = true }).ClickAsync();
await page.GetByLabel("Confirm Password").ClickAsync();
await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
await page.GetByText("Create a new account. Username The UserName field is required. Email The Email").ClickAsync();
await page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
await page.GetByRole(AriaRole.Heading, new() { Name = "Log in", Exact = true }).ClickAsync();
await page.GetByRole(AriaRole.Heading, new() { Name = "Use a local account to log in." }).ClickAsync();
await page.GetByPlaceholder("name").ClickAsync();
await page.GetByPlaceholder("password").ClickAsync();
await page.GetByText("Remember me?").ClickAsync();
await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
await page.GetByRole(AriaRole.Link, new() { Name = "public timeline" }).ClickAsync();
await page.GetByText("Jacqualine Gilcoine Starbuck").ClickAsync();
await page.GetByText("— 08/01/23 11.17.39").ClickAsync();
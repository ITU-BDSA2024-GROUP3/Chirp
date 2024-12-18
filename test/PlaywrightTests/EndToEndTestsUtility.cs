using System.Diagnostics;
using Microsoft.Playwright;

namespace PlaywrightTests;

public class EndToEndTestsUtility
{
    private static Process myProcess;

    public static async Task<Process> StartServer()
    {
        myProcess = new Process();
        try
        {
            myProcess = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "dotnet",
                Arguments = "run", //--project ./../../../../../src/ChirpWeb",
                WorkingDirectory = "./../../../../../src/ChirpWeb",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            myProcess.StartInfo = startInfo;
            myProcess.Start();

            await Task.Delay(10000);
        }
        catch (Exception e)
        {
            Console.WriteLine("Server did not start:" + e.Message);
        }

        return myProcess;
    }
    
    public static async Task UserLogIn(IPage page, string email, string userPassword)
    {
        await page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await page.GetByPlaceholder("name@name.com").ClickAsync();
        await page.GetByPlaceholder("name@name.com").FillAsync(email);
        await page.GetByPlaceholder("name@name.com").PressAsync("Tab");
        await page.GetByPlaceholder("password").FillAsync(userPassword);
        await page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
    }
    public static async Task UserLogOut(IPage page, string name)
    {
        
        await page.GetByRole(AriaRole.Link, new() { Name = $"logout [{name}]" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
    }

    public static async Task UserRegister(IPage page, string email, string userName, string password)
    {
   
        await page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();
        await page.GetByPlaceholder("name", new() { Exact = true }).ClickAsync();
        await page.GetByPlaceholder("name", new() { Exact = true }).FillAsync(userName);
        await page.GetByPlaceholder("name", new() { Exact = true }).PressAsync("Tab");
        await page.GetByPlaceholder("name@example.com").FillAsync(email);
        await page.GetByPlaceholder("name@example.com").PressAsync("Tab");
        await page.GetByLabel("Password", new() { Exact = true }).FillAsync(password);
        await page.GetByLabel("Password", new() { Exact = true }).PressAsync("Tab");
        await page.GetByLabel("Confirm Password").FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();
          

        
    }

    public static async Task UserDelete(IPage page, string userName, string password)
    {
        await page.GetByRole(AriaRole.Link, new() { Name = $"Manage {userName}'s account" }).ClickAsync();
        await page.GetByRole(AriaRole.Link, new() { Name = "Forget me!" }).ClickAsync();
        await page.GetByPlaceholder("Please enter your password.").ClickAsync();
        await page.GetByPlaceholder("Please enter your password.").FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my" }).ClickAsync();
        
    }
}
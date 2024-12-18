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
}
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

    public static async Task UserLogIn(IPage Page, string email, string userPassword)
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByPlaceholder("email").ClickAsync();
        await Page.GetByPlaceholder("name@name.com").FillAsync(email);
        await Page.GetByPlaceholder("password").ClickAsync();
        await Page.GetByPlaceholder("password").FillAsync(userPassword);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
    }
    public static async Task UserLogOut(IPage Page, string name)
    {
        
        await Page.GetByRole(AriaRole.Link, new() { Name = $"logout [{name}]" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Click here to Logout" }).ClickAsync();
    }
}
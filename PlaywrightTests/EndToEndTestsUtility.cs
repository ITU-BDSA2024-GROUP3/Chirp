using System.Diagnostics;

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
                WorkingDirectory = "./../../../../src/ChirpWeb",
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
}
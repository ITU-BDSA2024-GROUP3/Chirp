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
                //myProcess.StartInfo.UseShellExecute = false;
                // You can start any process, HelloWorld is a do-nothing example.
                //myProcess.StartInfo.FileName = "C:\\HelloWorld.exe";
                //myProcess.StartInfo.CreateNoWindow = true;
                var filename = "ChirpWeb";
                var cli = "dotnet run";
                myProcess.StartInfo.FileName = filename;
                myProcess.StartInfo.Arguments = cli;
                myProcess.Start();

        }
        catch (Exception e)
        {
            Console.WriteLine("Server did not start:" + e.Message);
        }
        return myProcess;
    }
    

}
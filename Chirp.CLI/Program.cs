using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using Chirp.CLI;
using SimpleDB;

class Program
{
    static void Main(string[] args)
    {
        const string dataPath = "./chirp_cli_db.csv";
        var cheepManager = new CSVDatabase<Cheep>(dataPath);

        if (args[0] == "read")
        {
            UserInterface.PrintCheeps(cheepManager.Read());
        }
        else if (args[0] == "cheep")
        {
            cheepManager.Store(new Cheep(Environment.UserName, args[1], ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()));
        }
        else
        {
            Console.WriteLine("Invalid command. Use 'read' to display cheeps or 'cheep <message>' to add a new cheep.");
        }
    }
}

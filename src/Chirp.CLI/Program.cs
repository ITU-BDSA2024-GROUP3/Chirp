using System.Runtime.CompilerServices;
using Chirp.CLI;
using DocoptNet;
using SimpleDB;
using UtilFunctions;

internal class Program
{
    /*
    const string usage = @"Chirp CLI version.

Usage:
  chirp read <limit>
  chirp cheep <message>
  chirp (-h | --help)
  chirp --version

Options:
  -h --help     Show this screen.
  --version     Show version.
";
    */

    private const string usage = @"Chirp CLI version.

Usage:
  chirp read
  chirp read <limit>
  chirp cheep <message>
  chirp (-h | --help)
  chirp --version

Options:
  -h --help     Show this screen.
  --version     Show version.
";


    private static int Main(string[] args)
    {
        //docopt is used to handle input from the console
        var parser = Docopt.CreateParser(usage).WithVersion("Chirp 0.0.1");
        return parser.Parse(args) switch
        {
            //goes through and runs different cases based on the console input
            IArgumentsResult<IDictionary<string, ArgValue>> { Arguments: var arguments } => Run(arguments),
            IHelpResult => ShowHelp(usage),
            IVersionResult { Version: var version } => ShowVersion(version),
            IInputErrorResult { Usage: var usage } => OnError(usage),
            var result => throw new SwitchExpressionException(result)
        };
    }

    private static int ShowHelp(string help)
    {
        Console.WriteLine("HELPING!!!! " + help);
        return 0;
    }

    private static int ShowVersion(string version)
    {
        Console.WriteLine(version);
        return 0;
    }

    private static int OnError(string usage)
    {
        Console.Error.WriteLine("ERROR " + usage);
        return 1;
    }

    private static int Run(IDictionary<string, ArgValue> arguments)
    {
        var cheepManager = CSVDatabase<Cheep>.instance;
        if (arguments["read"].IsTrue)
        {
            //prints the cheeps from the database in the console with or without limit of cheeps
            if (!arguments["<limit>"].IsNone)
            {
                int.TryParse(arguments["<limit>"].ToString(), out var limit);
                UserInterface.PrintCheeps(cheepManager.Read(limit));
            }
            else
            {
                UserInterface.PrintCheeps(cheepManager.Read());
            }
        }
        //adds the cheep message to the csvdatabase
        else if (arguments["cheep"].IsTrue)
        {
            var message = arguments["<message>"].ToString();
            cheepManager.Store(Util.CreateCheep(message));
        }
        else
        {
            Console.WriteLine("Invalid command. Use 'read' to display cheeps or 'cheep <message>' to add a new cheep.");
        }

        return 0;
    }
}
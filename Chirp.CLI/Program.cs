using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using Chirp.CLI;
using SimpleDB;
using DocoptNet;



class Program
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

    
    static int Main(string[] args)
    {
        var parser = Docopt.CreateParser(usage).WithVersion("Chirp 0.0.1");
        return parser.Parse(args) switch
        {
            IArgumentsResult<IDictionary<string, ArgValue>> { Arguments: var arguments } => Run(arguments),
            IHelpResult => ShowHelp(usage),
            IVersionResult { Version: var version } => ShowVersion(version),
            IInputErrorResult { Usage: var usage } => OnError(usage),
            var result => throw new System.Runtime.CompilerServices.SwitchExpressionException(result)
        };
    }
    
    static int ShowHelp(string help) { Console.WriteLine("HELPING!!!! " + help); return 0; }
    static int ShowVersion(string version) { Console.WriteLine(version); return 0; }
    static int OnError(string usage) { Console.Error.WriteLine("ERROR " + usage); return 1; }

    static int Run(IDictionary<string, ArgValue> arguments)
    {
        /*
        Console.WriteLine("RUN!");
        foreach (var (key, value) in arguments)
            Console.WriteLine("{0} = {1}", key, value);
        return 0;
        */
        
        const string dataPath = "./chirp_cli_db.csv";
        var cheepManager = new CSVDatabase<Cheep>(dataPath);

        if (arguments["read"].IsTrue)
        {
            /*
            // Print all methods in type
            System.Reflection.MethodInfo[] info = typeof(ArgValue).GetMethods();
            foreach (System.Reflection.MethodInfo method in info)
            {
                
                string s = $"{method.ReturnType} {method.Name}(";
                System.Reflection.ParameterInfo[] parameters = method.GetParameters();
                foreach (System.Reflection.ParameterInfo parameter in parameters)
                {
                    s += parameter.ToString + ", ";
                }
                Console.WriteLine(method.ToString());
            } 
            */

            Int32.TryParse(arguments["<limit>"].ToString(), out var limit);
            UserInterface.PrintCheeps(cheepManager.Read(limit));
        }
        else if (arguments["cheep"].IsTrue)
        {
            var message = arguments["<message>"].ToString();
            cheepManager.Store(new Cheep(Environment.UserName, message, ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()));
        }
        else
        {
            Console.WriteLine("Invalid command. Use 'read' to display cheeps or 'cheep <message>' to add a new cheep.");
        }

        return 0;
    }
}

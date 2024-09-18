using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using Chirp.CLI;
using SimpleDB;
using UtilFunctions;
using DocoptNet;


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
  chirp read
  chirp read <limit>
  chirp cheep <message>
  chirp (-h | --help)
  chirp --version

Options:
  -h --help     Show this screen.
  --version     Show version.
";

var arguments = new Docopt().Apply(usage, args, version: "0.0.1", exit: true);

var cheepManager = CSVDatabase<Cheep>.instance;


if ((bool)arguments["read"].Value)
{
    int? limit = null;
    if (!arguments["<limit>"].IsNullOrEmpty)
    {
        limit = Convert.ToInt32(arguments["<limit>"].Value);
    }
    UserInterface.PrintCheeps(cheepManager.Read(limit));
}

if ((bool)(arguments["cheep"].Value))
{
    var message = arguments["<message>"].Value.ToString();
    cheepManager.Store(Util.CreateCheep(message));
}
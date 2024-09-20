using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;
using Chirp.CLI;
using SimpleDB;
using UtilFunctions;
using DocoptNet;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;


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

if ((bool)arguments["read"].Value)
{
    int? limit = null;
    if (!arguments["<limit>"].IsNullOrEmpty)
    {
        limit = Convert.ToInt32(arguments["<limit>"].Value);
    }

    // Create an HTTP client object
    var baseURL = "http://localhost:5132";
    using HttpClient client = new();
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.BaseAddress = new Uri(baseURL);
    
    // Send an asynchronous HTTP GET request and automatically construct a Cheep object from the
    // JSON object in the body of the response
    var requestURI = $"cheeps";
    if(limit != null) requestURI += $"?limit={limit}";
    
    var cheeps = await client.GetFromJsonAsync<IEnumerable<Cheep>>(requestURI);
    
    UserInterface.PrintCheeps(cheeps);
}

if ((bool)(arguments["cheep"].Value))
{
    var message = arguments["<message>"].Value.ToString();
    
    Cheep cheep = Util.CreateCheep(message);
    
    var baseURL = "http://localhost:5132";
    using HttpClient client = new();
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.BaseAddress = new Uri(baseURL);
    
    var requestURI = $"cheep";
    requestURI += $"?message={message}";
    
    
    CancellationTokenSource cts = new();
    CancellationToken cancellationToken = cts.Token;
    
    var temp = await client.PostAsJsonAsync(requestURI, cheep, cancellationToken);
  
    //cheepManager.Store(Util.CreateCheep(message));
}
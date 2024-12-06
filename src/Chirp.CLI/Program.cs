using System.Runtime.CompilerServices;
using Chirp.CSVDBService;
using DocoptNet;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Chirp.CLI;


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
//docopt is used to read input from the console
var arguments = new Docopt().Apply(usage, args, version: "0.0.1", exit: true);

if ((bool)arguments["read"].Value)
{
    int? limit = null;
    if (!arguments["<limit>"].IsNullOrEmpty)
    {
        limit = Convert.ToInt32(arguments["<limit>"].Value);
    }

    // Create an HTTP client object
    var baseURL = "https://bdsagroup3chirpremotedb.azurewebsites.net/";
    //var baseURL = "http://localhost:5132";
    using HttpClient client = new();
    /*client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.*/
    client.BaseAddress = new Uri(baseURL);
    
    // Send an asynchronous HTTP GET request and automatically construct a Cheep object from the
    // JSON object in the body of the response
    var requestURI = $"cheeps";
    if(limit != null) requestURI += $"?limit={limit}";
    
    var cheepsRes = await client.GetAsync(requestURI);
    
    var json = await cheepsRes.Content.ReadAsStringAsync();
    var result = string.IsNullOrEmpty(json) ? null : JsonObject.Parse(json);
    
    
    //List<Cheep> cheeps = await cheepsRes.Content.ReadFromJsonAsync<List<Cheep>>();

    //var cheeps = await client.GetAsync(requestURI);

    /*
    Console.WriteLine(cheepsRes.StatusCode == (HttpStatusCode)200);
    
    Console.WriteLine(cheeps);

    if (cheeps != null) UserInterface.PrintCheeps(cheeps);
    */
    Console.WriteLine(json);
    Console.WriteLine(cheepsRes.ToString());
    Console.WriteLine("json");

}

if ((bool)(arguments["cheep"].Value))
{
    var message = arguments["<message>"].Value.ToString();
    
    var cheep = Util.CreateCheep(message);
    
    var baseURL = "https://bdsagroup3chirpremotedb.azurewebsites.net/";
    //var baseURL = "http://localhost:5132";
    using HttpClient client = new();
    /*client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    */client.BaseAddress = new Uri(baseURL);
    
    var requestURI = $"/cheep";
    requestURI += "?message={message}";
    
    
    using var response = await client.PutAsJsonAsync($"/cheep", cheep);
    //response.EnsureSuccessStatusCode();
            
    Console.WriteLine($"Post successful: {cheep.ToString()}");
    
    using var temp = await client.PostAsJsonAsync(requestURI, cheep);
    
    // following can be used to test what and if the statuscode of our cheeps is/works
    Console.WriteLine(temp.StatusCode == (HttpStatusCode)200);
    Console.WriteLine(temp.StatusCode);

  
    //cheepManager.Store(Util.CreateCheep(message));
}
// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

const string dataPath = "./chirp_cli_db.csv";

if (args[0] == "read")
{
    StreamReader reader = new StreamReader(dataPath);
    reader.ReadLine();
    while (!reader.EndOfStream)
    {
        // Regex taken from: https://stackoverflow.com/questions/6542996/how-to-split-csv-whose-columns-may-contain-comma
        string[] words = Regex.Split(reader.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
        long unixTimeSeconds = long.Parse(words[2]);
        DateTimeOffset timeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds);
        string time = $"{timeOffset.Day}/{timeOffset.Month}/{timeOffset.Year}";
        Console.WriteLine($"{words[0]} @ {time} : {words[1]}");
    }
    reader.Close();
}
else if(args[0] == "cheep")
{
    //easy unix conversion taken from: https://stackoverflow.com/a/35425123
    using (StreamWriter sw = File.AppendText(dataPath))
    {
     sw.WriteLine(Environment.UserName + "," + '"' + args[1]  + '"' + "," + ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
    }
}
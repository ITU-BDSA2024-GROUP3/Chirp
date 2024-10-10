using Chirp.CSVDBService;

namespace Chirp.CLI;

public static class UserInterface
{
    //prints all the cheeps stored in the database
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        Console.WriteLine("Writing cheeps");
        foreach (var cheep in cheeps)
            //Console.WriteLine($"{cheep.Author} @ {ConvertTime(cheep.Timestamp)} : {cheep.Message}");
            Console.WriteLine(cheep.ToString());
    }

    //converts the unix time stamp to a timeset day:month:year
    public static string ConvertTime(long timestamp) //private(?)
    {
        var timeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        return $"{timeOffset.Day}/{timeOffset.Month}/{timeOffset.Year}";
    }
}
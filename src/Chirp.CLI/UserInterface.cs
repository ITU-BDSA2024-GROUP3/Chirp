using SimpleDB;

namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (Cheep cheep in cheeps)
        {
            Console.WriteLine($"{cheep.Author} @ {ConvertTime(cheep.Timestamp)} : {cheep.Message}");
        }
        
    }

    public static string ConvertTime(long timestamp)//private(?)
    {
        DateTimeOffset timeOffset = DateTimeOffset.FromUnixTimeSeconds(timestamp);
        return $"{timeOffset.Day}/{timeOffset.Month}/{timeOffset.Year}";
    }
    
}

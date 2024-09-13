using SimpleDB;

namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheeps(IEnumerable<Cheep> cheeps)
    {
        foreach (Cheep cheep in cheeps){
            DateTimeOffset timeOffset = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp);
            string time = $"{timeOffset.Day}/{timeOffset.Month}/{timeOffset.Year}";
            Console.WriteLine($"{cheep.Author} @ {time} : {cheep.Message}");
        }
        
    }
}

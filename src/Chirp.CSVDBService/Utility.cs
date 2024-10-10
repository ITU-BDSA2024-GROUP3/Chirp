namespace Chirp.CSVDBService;

public record Cheep(string Author, string Message, long Timestamp);

public static class Util
{
    public static Cheep CreateCheep(string message)
    {
        return new Cheep(Environment.UserName, message, ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
    }
}





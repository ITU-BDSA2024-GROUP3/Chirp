using Chirp.Razor;

public record CheepViewModel(string Author, string Message, Int64 Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps(int page);
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    
    public List<CheepViewModel> GetCheeps(int page)
    {
        DBFacade facade = new DBFacade();
        return facade.ReadCheeps(page);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        DBFacade facade = new DBFacade();

        return facade.GetCheepsFromAuthor(author);
    }
    
    public static string UnixTimeStampToDateTimeString(Int64 unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

    
}
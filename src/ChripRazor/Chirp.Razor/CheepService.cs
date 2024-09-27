using Chirp.Razor;

public record CheepViewModel(string Author, string Message, string Timestamp);

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

    
}
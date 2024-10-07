using Chirp.Razor;
using Chirp.Razor.DomainModel;

//public record CheepViewModel(string Author, string Message, Int64 Timestamp);

public interface ICheepService
{
    public Task<List<CheepDTO>> GetCheeps(int page);
    public Task<AuthorDTO> GetAuthor(int id);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int userId, int page);
}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _repository;
    private readonly DBFacade facade;

    public CheepService(ICheepRepository repository)
    {
        _repository = repository;
        facade = new DBFacade(_repository);
    }
    
    public Task<List<CheepDTO>> GetCheeps(int page)
    {
        return facade.ReadCheeps(page);
    }

    public Task<AuthorDTO> GetAuthor(int id)
    {
        return facade.GetAuthor(id);
    }
    
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int userId, int page)
    {
        // filter by the provided author name

        return facade.GetCheepsFromAuthor(userId, page);
    }
    
    public static string UnixTimeStampToDateTimeString(Int64 unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        // returns GMT Timezone 
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

    
}
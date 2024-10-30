using ChirpCore;
using ChirpCore.DomainModel;

namespace ChirpWeb;
//public record CheepViewModel(string Author, string Message, Int64 Timestamp);

public interface ICheepService
{
    public Task<int> CreateCheep(CheepDTO newMessage);
    public Task<List<CheepDTO>> GetCheeps(int page);
    public Task<AuthorDTO> GetAuthor(int id);
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int userId, int page);
    public Task<AuthorDTO> ReadAuthorByEmail(string userEmail);
    public Task<AuthorDTO> ReadAuthorByName(string userName);
    
    public Task<int> GetAuthorCount();

}

public class CheepService : ICheepService
{
    private readonly ICheepRepository _repository;

    public CheepService(ICheepRepository repository)
    {
        _repository = repository;
    }

    public Task<int> CreateCheep(CheepDTO newMessage)
    {
        return _repository.CreateCheep(newMessage);
    }

    public Task<List<CheepDTO>> GetCheeps(int page)
    {
        return _repository.ReadCheeps(page, null);
    }

    public Task<AuthorDTO> GetAuthor(int id)
    {
        return _repository.ReadAuthorById(id);
    }
    
    public Task<AuthorDTO> ReadAuthorByEmail(string userEmail)
    {
        return _repository.ReadAuthorByEmail(userEmail);
    }
    
    public Task<int> GetAuthorCount()
    {
        return _repository.GetAuthorCount();
    }
    
    public Task<AuthorDTO> ReadAuthorByName(string username)
    {
        return _repository.ReadAuthorByName(username);
    }
    
    public Task<List<CheepDTO>> GetCheepsFromAuthor(int userId, int page)
    {
        // filter by the provided author name

        return _repository.ReadCheeps(page, userId);
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
using ChirpCore.DomainModel;

namespace ChirpCore;

public interface ICheepRepository
{
    public Task<int> CreateCheep(CheepDTO newMessage);
    public Task<List<CheepDTO>> ReadCheeps(int page, int? userId);
    public Task<List<CheepDTO>> ReadAllCheeps(int? userId);
    public Task<List<CheepDTO>> ReadFollowedCheeps(int page, int? UserId);
    public Task<List<AuthorDTO>> ReadFollowing(int UserId);
    public Task<int> UpdateCheep(Cheep updatedMessage);
    public Task<AuthorDTO> ReadAuthorDTOById(int id);
    public Task<Author> ReadAuthorByName(string name);
    
    public Task<Author> ReadAuthorById(int id);
    public Task<AuthorDTO> ReadAuthorDTOByEmail(string email);
    public Task<Author> ReadAuthorByEmail(string email);
    public Task<Author> CreateAuthor(string name, string email, int userId);
    public Task<int> GetAuthorCount();
    public Task<string> GetNameByEmail(string emailAddress);
    public Task<int> Follow(int wantToFollow, int wantToBeFollowed);
    public Task<int> Unfollow( int wantToUnfollow, int wantToBeUnfollowed);
    
}
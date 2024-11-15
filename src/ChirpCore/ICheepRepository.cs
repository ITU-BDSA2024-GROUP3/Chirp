using ChirpCore.DomainModel;

namespace ChirpCore;

public interface ICheepRepository 
{
    public Task<int> CreateCheep(CheepDTO newMessage);
    public Task<List<CheepDTO>> ReadCheeps(int page, int? userId);
    public Task<int> UpdateCheep(Cheep updatedMessage);
    public Task<AuthorDTO> ReadAuthorDTOById(int id);
    public Task<AuthorDTO> ReadAuthorByName(string name);
    public Task<AuthorDTO> ReadAuthorByEmail(string email);
    public Task<Author> CreateAuthor(string name, string email, int userId);
    public Task<int> GetAuthorCount();
    public Task<string> GetNameByEmail(string emailAddress);
    public Task Follow(Author wantToFollow, Author wantToBeFollowed);
    public Task Unfollow( Author wantToUnfollow, Author wantToBeUnfollowed);
    
}
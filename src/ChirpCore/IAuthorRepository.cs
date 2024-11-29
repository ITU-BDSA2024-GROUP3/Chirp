using ChirpCore.DomainModel;

namespace ChirpCore;

public interface IAuthorRepository
{
    public Task<AuthorDTO?> ReadAuthorDTOById(int id);
    public Task<Author?> ReadAuthorByName(string name);
    public Task<Author?> ReadAuthorById(int id);
    public Task<AuthorDTO?> ReadAuthorDTOByEmail(string email);
    public Task<Author?> ReadAuthorByEmail(string email);
    public Task<Author> CreateAuthor(string name, string email, int userId);
    public Task<int> GetAuthorCount();
    public Task<string?> GetNameByEmail(string emailAddress);
    public Task<int> Follow(int wantToFollow, int wantToBeFollowed);
    public Task<int> Unfollow( int wantToUnfollow, int wantToBeUnfollowed);
    Task<AuthorDTO?> ReadAuthorDTOByName(string name);
}
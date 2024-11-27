using ChirpCore.DomainModel;

namespace ChirpCore;

public interface ICheepRepository
{
    public Task<int> CreateCheep(CheepDTO newMessage);
    public Task<List<CheepDTO>> ReadCheeps(int page, int? userId);
    public Task<List<CheepDTO>> ReadFollowedCheeps(int page, int? UserId);
    public Task<List<CheepDTO>> ReadAllCheeps(int? userId);
    public Task<List<AuthorDTO>> ReadFollowing(int UserId);
    public Task<int> UpdateCheep(Cheep updatedMessage);
    
  
    
}
using ChirpCore.DomainModel;

namespace ChirpCore;

public interface ICheepRepository
{
    public Task<int> CreateCheep(CheepDTO newMessage);
    public List<CheepDTO> ReadCheeps(int page, int? userId);
    public List<CheepDTO> ReadFollowedCheeps(int page, int? UserId);
    public List<CheepDTO> ReadAllCheeps(int? userId);
    public Task<List<AuthorDTO>> ReadFollowingAsync(int UserId);
    public Task<int> UpdateCheep(Cheep updatedMessage);
    
  
    
}
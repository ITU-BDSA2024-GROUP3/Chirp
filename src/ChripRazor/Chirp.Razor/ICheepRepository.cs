using Chirp.Razor.DomainModel;

namespace Chirp.Razor;

public interface ICheepRepository 
{
    public Task<int> CreateCheep(CheepDTO newMessage);
    public Task<List<CheepDTO>> ReadCheeps(int page, int? userId);
    public Task<int> UpdateCheep(Cheep updatedMessage);
}
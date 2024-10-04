using Chirp.Razor.DomainModel;

namespace Chirp.Razor;

public interface ICheepRepository 
{
    public Task<int> CreateCheep(MessageDTO newMessage);
    public Task<List<MessageDTO>> ReadCheeps(int page, int? userId);
    public Task<int> UpdateCheep(Message updatedMessage);
}
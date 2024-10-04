using Chirp.Razor.DomainModel;

namespace Chirp.Razor;

public interface ICheepRepository 
{
    public Task<int> CreateCheep(MessageDTO newMessage);
    public Task<List<MessageDTO>> ReadCheeps();
    public Task<int> UpdateCheep(Message updatedMessage);
}
using Chirp.Razor.DomainModel;

namespace Chirp.Razor;

public interface ICheepRepository 
{
    public Task<int> CreateCheep(Message newMessage);
    public Task<List<Message>> ReadCheeps();
    public Task UpdateCheep(Message updatedMessage);
}
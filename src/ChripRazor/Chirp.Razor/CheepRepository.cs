using Chirp.Razor.DomainModel;

namespace Chirp.Razor;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    public CheepRepository(ChirpDBContext context)
    {
        _dbContext = context;
    }
    
    public Task CreateCheep(Message newMessage)
    {
        throw new NotImplementedException();
    }

    public Task<List<Message>> ReadCheeps()
    {
        throw new NotImplementedException();
    }

    public Task UpdateCheep(Message updatedMessage)
    {
        throw new NotImplementedException();
    }
}
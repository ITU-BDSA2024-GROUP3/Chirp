using Chirp.Razor.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;

    public CheepRepository(ChirpDBContext context)
    {
        _dbContext = context;
    }

    public async Task<int> CreateCheep(MessageDTO newMessage)
    {
        Message message = new() { Text = newMessage.Text, User = newMessage.User, Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()};
        var queryResult = await _dbContext.Messages.AddAsync(message); // does not write to the database!

        await _dbContext.SaveChangesAsync(); // persist the changes in the database
        return queryResult.Entity.MessageId;
    }

    public async Task<List<MessageDTO>> ReadCheeps(int page, int? userId )
    {
        // Formulate the query - will be translated to SQL by EF Core
        IQueryable<MessageDTO> query;
        if (userId != null)
        {
            query = _dbContext.Messages.Where(message => message.UserId == userId).Select(message => new MessageDTO()
                { Text = message.Text, User = message.User }).Skip((page - 1) * 32).Take(32);
        }
        else
        {
            query = _dbContext.Messages.Select(message => new MessageDTO()
                { Text = message.Text, User = message.User }).Skip((page - 1) * 32).Take(32);
        }
        
        // Execute the query
        var result = await query.ToListAsync();
        
        return result;
    }

    public async Task<int> UpdateCheep(Message updatedMessage)
    {
        var message = await _dbContext.Messages.FindAsync(updatedMessage.MessageId);
        if (message is null)
        {
            throw new Exception("Unable to find the recipe");
        }
        message.Text = updatedMessage.Text;
        message.Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        return message.MessageId;
    }
}
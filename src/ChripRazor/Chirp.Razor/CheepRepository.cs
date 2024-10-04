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

    public async Task<int> CreateCheep(CheepDTO newMessage)
    {
        Cheep message = new() { Text = newMessage.Text, Author = newMessage.Author, TimeStamp = DateTime.Now};
        var queryResult = await _dbContext.Cheeps.AddAsync(message); // does not write to the database!

        await _dbContext.SaveChangesAsync(); // persist the changes in the database
        return queryResult.Entity.CheepId;
    }

    public async Task<List<CheepDTO>> ReadCheeps(int page, int? authorId )
    {
        // Formulate the query - will be translated to SQL by EF Core
        IQueryable<CheepDTO> query;
        if (authorId != null)
        {
            query = _dbContext.Cheeps.Where(message => message.AuthorId == authorId).Select(message => new CheepDTO()
                { Text = message.Text, Author = message.Author }).Skip((page - 1) * 32).Take(32);
        }
        else
        {
            query = _dbContext.Cheeps.Select(message => new CheepDTO()
                { Text = message.Text, Author = message.Author }).Skip((page - 1) * 32).Take(32);
        }
        
        // Execute the query
        var result = await query.ToListAsync();
        
        return result;
    }

    public async Task<int> UpdateCheep(Cheep updatedMessage)
    {
        var message = await _dbContext.Cheeps.FindAsync(updatedMessage.CheepId);
        if (message is null)
        {
            throw new Exception("Unable to find the recipe");
        }
        message.Text = updatedMessage.Text;
        message.TimeStamp = DateTime.Now;
        return message.CheepId;
    }

    public async Task<AuthorDTO> ReadAuthor(int id)
    {
        IQueryable<AuthorDTO> query = _dbContext.Authors.Where(author => author.AuthorId == id)
            .Select(author => new AuthorDTO() {Name = author.Name})
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
}
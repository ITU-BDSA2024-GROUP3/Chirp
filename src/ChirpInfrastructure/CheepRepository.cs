using ChirpCore;
using ChirpCore.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace ChirpInfrastructure;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;

    public CheepRepository(ChirpDBContext context)
    {
        _dbContext = context;
    }

    public async Task<int> CreateCheep(CheepDTO newMessage)
    {
        Cheep message = new() { Text = newMessage.Text, Author = newMessage.Author, TimeStamp = DateTime.Now };
        var queryResult = await _dbContext.Cheeps.AddAsync(message); // does not write to the database!
        
        
        await _dbContext.SaveChangesAsync(); // persist the changes in the database
        
        newMessage.Author.Cheeps.Add(message);

        return queryResult.Entity.CheepId;
    }

    public async Task<List<CheepDTO>> ReadCheeps(int page, int? UserId)
    {
        // Formulate the query - will be translated to SQL by EF Core
        IQueryable<CheepDTO> query;
        if (UserId != null)
        {
            query = Queryable.Where<Cheep>(_dbContext.Cheeps, message => message.UserId == UserId).Select(message => new CheepDTO()
                    { Text = message.Text, Author = message.Author, TimeStamp = message.TimeStamp.ToUnixTimeSeconds() })
                .Skip((page - 1) * 32).Take(32);
        }
        else
        {
            query = _dbContext.Cheeps.Select(message => new CheepDTO()
                    { Text = message.Text, Author = message.Author, TimeStamp = message.TimeStamp.ToUnixTimeSeconds() })
                .Skip((page - 1) * 32).Take(32);
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

    public async Task<AuthorDTO> ReadAuthorById(int id)
    {
        IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.UserId == id)
            .Select(author => new AuthorDTO() { Name = author.Name, UserId = author.UserId })
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<AuthorDTO> ReadAuthorByName(string name)
    {
        IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Name == name)
            .Select(author => new AuthorDTO() { Name = author.Name, UserId = author.UserId})
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<AuthorDTO> ReadAuthorByEmail(string email)
    {
        IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Email == email)
            .Select(author => new AuthorDTO() { Name = author.Name, UserId = author.UserId  })
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<Author> CreateAuthor(string name, string email, int UserId)
    {
        Author author = new() { UserId = UserId, Name = name, Email = email, Cheeps = new List<Cheep>() };
        var queryResult = await _dbContext.Authors.AddAsync(author); // does not write to the database!

        await _dbContext.SaveChangesAsync();
        
        return queryResult.Entity;
    }

    public async Task<int> GetAuthorCount()
    {
        IQueryable<AuthorDTO> query = _dbContext.Authors.Select(author => new AuthorDTO() { Name = author.Name })
            .OrderDescending().Take(1);
        Console.WriteLine(query.Count());
        return await query.CountAsync();
    }
}
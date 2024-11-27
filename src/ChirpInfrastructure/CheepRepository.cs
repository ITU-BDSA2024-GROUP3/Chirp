using System.Runtime.InteropServices.JavaScript;
using ChirpCore;
using ChirpCore.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace ChirpInfrastructure;

public class CheepRepository : ICheepRepository
{
    private readonly ChirpDBContext _dbContext;
    private int _nextCheepId;

    public CheepRepository(ChirpDBContext context)
    {
        _dbContext = context;
        _nextCheepId = 0;
    }

    public async Task<int> CreateCheep(CheepDTO newMessage)
    {
        Author cheepAuthor = ReadAuthorById(newMessage.UserId).Result;

        Cheep message = new()
        {
            CheepId = _nextCheepId++, UserId = newMessage.UserId, Text = newMessage.Text, TimeStamp = DateTime.Now,
            Author = cheepAuthor
        };
        var queryResult = _dbContext.Cheeps.Add(message); // does not write to the database!
        cheepAuthor.Cheeps.Add(message);


        await _dbContext.SaveChangesAsync(); // persist the changes in the database

        Console.WriteLine($"Store Cheep message = {message.Text} and UserId = {message.Author.UserId}");

        return queryResult.Entity.CheepId;
    }

    public async Task<List<AuthorDTO>> ReadFollowing(int UserId)
    {
        if (UserId == null)
        {
            throw new Exception("No UserID provided!");
        }

        Author author = ReadAuthorById(UserId).Result;
        if (author == null)
        {
            throw new Exception($"No Author with ID {UserId}!");
        } else if (author.FollowingList == null)
        {
            throw new Exception("No Following list provided!");
        }
        
        List<AuthorDTO> followers = new List<AuthorDTO>();

        foreach (var followerId in author.FollowingList)
        {
            
            Author tempAuthor = ReadAuthorById(followerId).Result;
            
            AuthorDTO authorDto = new AuthorDTO() { Name = tempAuthor.Name, UserId = followerId };
            
            followers.Add(authorDto);
        }

        return followers;
    }

    
    public async Task<List<CheepDTO>> ReadFollowedCheeps(int page, int? UserId)
    {
        if (UserId == null)
        {
            throw new Exception("No UserID provided!");
        }

        Author author = ReadAuthorById((int)UserId).Result;
        if (author == null)
        {
            throw new Exception($"No Author with ID {UserId}!");
        } else if (author.FollowingList == null)
        {
            throw new Exception("No Following list provided!");
        }
        
        IQueryable<CheepDTO> query = _dbContext.Cheeps
            .Where(message => author.FollowingList.Contains(message.UserId) || message.UserId == UserId)
            .Include(cheep => cheep.Author)
            .Select(message => new CheepDTO() {
                Text = message.Text,
                UserId = message.Author.UserId,
                AuthorName = message.Author.Name,
                TimeStamp = message.TimeStamp.ToUnixTimeSeconds()
            })
            .AsEnumerable()
            .OrderByDescending(dto => dto.TimeStamp)
            .AsQueryable()
            .Skip((page - 1) * 32)
            .Take(32);

        return query.ToList();
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

    
    public async Task<List<CheepDTO>> ReadCheeps(int page, int? UserId)
    {
        // Formulate the query - will be translated to SQL by EF Core
        IQueryable<CheepDTO> query;
        if (UserId != null)
        {
            query = Queryable.Where<Cheep>(_dbContext.Cheeps, message => message.UserId == UserId)
                .Select(message => new CheepDTO() {
                    Text = message.Text, 
                    UserId = message.Author.UserId, 
                    AuthorName = message.Author.Name,
                    TimeStamp = message.TimeStamp.ToUnixTimeSeconds()
                })
                .AsEnumerable()
                .OrderByDescending(dto => dto.TimeStamp)
                .AsQueryable()
                .Skip((page - 1) * 32)
                .Take(32);
        }
        else
        {
            query = _dbContext.Cheeps
                .Select(message => new CheepDTO() {
                    Text = message.Text,
                    UserId = message.Author.UserId,
                    AuthorName = message.Author.Name,
                    TimeStamp = message.TimeStamp.ToUnixTimeSeconds()
                })
                .AsEnumerable()
                .OrderByDescending(dto => dto.TimeStamp)
                .AsQueryable()
                .Skip((page - 1) * 32)
                .Take(32);
        }

        // Execute the query
        var result = query.ToList();

        return result;
    }
    
    public async Task<List<CheepDTO>> ReadAllCheeps(int? UserId)
    {
        // Formulate the query - will be translated to SQL by EF Core
        IQueryable<CheepDTO> query;
        if (UserId != null)
        {
            query = Queryable.Where<Cheep>(_dbContext.Cheeps, message => message.UserId == UserId)
                .Select(message => new CheepDTO()
                {
                    Text = message.Text,
                    UserId = message.Author.UserId,
                    AuthorName = message.Author.Name,
                    TimeStamp = message.TimeStamp.ToUnixTimeSeconds()
                })
                .AsEnumerable()
                .OrderByDescending(dto => dto.TimeStamp)
                .AsQueryable();
        }
        else
        {
            query = _dbContext.Cheeps
                .Select(message => new CheepDTO()
                {
                    Text = message.Text,
                    UserId = message.Author.UserId,
                    AuthorName = message.Author.Name,
                    TimeStamp = message.TimeStamp.ToUnixTimeSeconds()
                })
                .AsEnumerable()
                .OrderByDescending(dto => dto.TimeStamp)
                .AsQueryable();
        }

        // Execute the query
        var result = query.ToList();

        return result;
    }

    public async Task<Author> ReadAuthorById(int id)
    {
        IQueryable<Author> query = Queryable.Where(_dbContext.Authors, author => author.UserId == id)
            .Select(author => author)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }

    

    
    public static string UnixTimeStampToDateTimeString(Int64 unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        // returns GMT Timezone 
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss");
    }

   

}
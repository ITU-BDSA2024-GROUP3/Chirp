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
        Author? cheepAuthor = ReadAuthorById(newMessage.UserId).Result;

        if (cheepAuthor == null)
        {
            throw new Exception("User not recognized!");
        }

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

    public async Task<List<AuthorDTO>> ReadFollowingAsync(int UserId)
    {
        Author? author = await ReadAuthorById(UserId);
        if (author == null)
        {
            throw new Exception($"No Author with ID {UserId}!");
        }
        else if (author.FollowingList == null)
        {
            throw new Exception("No Following list provided!");
        }

        List<AuthorDTO> followers = new List<AuthorDTO>();

        List<int> removeTheseIds = new List<int>();

        foreach (var followerId in author.FollowingList)
        {
            Author? tempAuthor = await ReadAuthorById(followerId);
            if (tempAuthor == null)
            {
                removeTheseIds.Add(followerId);
            }
            else
            {
                AuthorDTO authorDto = tempAuthor.ToDTO();

                followers.Add(authorDto);
            }
        }

        foreach (var followerId in removeTheseIds)
        {
            await Unfollow(author.UserId, followerId);
        }

        return followers;
    }


    public List<CheepDTO> ReadFollowedCheeps(int page, int? UserId)
    {
        if (UserId == null)
        {
            throw new Exception("No UserID provided!");
        }

        Author? author = ReadAuthorById((int)UserId).Result;
        if (author == null)
        {
            throw new Exception($"No Author with ID {UserId}!");
        }
        else if (author.FollowingList == null)
        {
            throw new Exception("No Following list provided!");
        }

        var query = _dbContext.Cheeps
            .Where(message => author.FollowingList.Contains(message.UserId) || message.UserId == UserId)
            .Include(cheep => cheep.Author)
            .Select(message => message.ToDTO())
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


    public List<CheepDTO> ReadCheeps(int page, int? UserId)
    {
        // Formulate the query - will be translated to SQL by EF Core
        IQueryable<CheepDTO> query;
        if (UserId != null)
        {
            query = Queryable.Where<Cheep>(_dbContext.Cheeps, message => message.UserId == UserId)
                .Include(message => message.Author)
                .Select(message => message.ToDTO())
                .AsEnumerable()
                .OrderByDescending(dto => dto.TimeStamp)
                .AsQueryable()
                .Skip((page - 1) * 32)
                .Take(32);
        }
        else
        {
            query = _dbContext.Cheeps
                .Include(message => message.Author)
                .Select(message => message.ToDTO())
                .AsEnumerable()
                .OrderByDescending(dto => dto.TimeStamp)
                .AsQueryable()
                .Skip((page - 1) * 32)
                .Take(32);
        }

        return query.ToList();
    }

    public List<CheepDTO> ReadAllCheeps(int? UserId)
    {
        // Formulate the query - will be translated to SQL by EF Core
        IQueryable<CheepDTO> query;
        if (UserId != null)
        {
            query = Queryable.Where<Cheep>(_dbContext.Cheeps, message => message.UserId == UserId)
                .Include(message => message.Author)
                .Select(message => message.ToDTO())
                .AsEnumerable()
                .OrderByDescending(dto => dto.TimeStamp)
                .AsQueryable();
        }
        else
        {
            query = _dbContext.Cheeps
                .Include(message => message.Author)
                .Select(message => message.ToDTO())
                .AsEnumerable()
                .OrderByDescending(dto => dto.TimeStamp)
                .AsQueryable();
        }

        return query.ToList();
    }

    public async Task<Author?> ReadAuthorById(int id)
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

    public async Task<int> Unfollow(int wantToUnfollow, int wantToBeUnfollowed)
    {
        if (wantToUnfollow == wantToBeUnfollowed)
        {
            throw new Exception($"Cannot unfollow yourself!");
        }

        Author? wantToUnfollowAuthor = ReadAuthorById(wantToUnfollow).Result;

        if (wantToUnfollowAuthor == null)
        {
            throw new Exception("User not recognized!");
        }

        if (!wantToUnfollowAuthor.FollowingList.Contains(wantToBeUnfollowed))
        {
            throw new Exception($"Cannot unfollow - Not following this author");
        }

        wantToUnfollowAuthor.FollowingList.Remove(wantToBeUnfollowed);
        await _dbContext.SaveChangesAsync();
        return wantToUnfollowAuthor.FollowingList.Count;
    }
    
    public async Task<int> AmountOfLikes(int cheepId)
    {
        Cheep cheep = await ReadCheepByCheepId(cheepId);
        
        if (cheep == null)
        {
            throw new Exception($"Cheep does not exist, AmountOfLikes in cheepRepo");
        }

        var removeThese = new List<int>();
        if (cheep.AuthorLikeList == null)
        {
            cheep.AuthorLikeList = new List<int>();
        }
        foreach (var authorid in cheep.AuthorLikeList)
        {
            Author tempAuthor = await ReadAuthorById(authorid);
            if (tempAuthor == null)
            {
                removeThese.Add(authorid);
            }
        }

        foreach (var authorid in removeThese)
        {
            cheep.AuthorLikeList.Remove(authorid);
        }

        await _dbContext.SaveChangesAsync(); // persist the changes in the database
        
        return cheep.AuthorLikeList.Count;
    }
    
    public async Task<int> UnLikeCheep(int cheepid, int userId)
    {
        if (ReadCheepByCheepId(cheepid).Result==null || ReadAuthorById(userId).Result == null)
        {
            throw new Exception($"User or cheep does not exist");
        }
        if (!ReadCheepByCheepId(cheepid).Result.AuthorLikeList.Contains(userId))
        {
            throw new Exception($"Did not like this cheep!");
        }
        ReadCheepByCheepId(cheepid).Result.AuthorLikeList.Remove(userId);

        await _dbContext.SaveChangesAsync();
        return ReadCheepByCheepId(cheepid).Result.AuthorLikeList.Count;
    }
    public async Task<int> LikeCheep(int cheepid, int userId)
    {
        if (ReadCheepByCheepId(cheepid).Result==null || ReadAuthorById(userId).Result == null)
        {
            throw new Exception($"User or cheep does not exist");
        }
        if (ReadCheepByCheepId(cheepid).Result.AuthorLikeList.Contains(userId))
        {
            throw new Exception($"Already like this cheep!");
        }
        ReadCheepByCheepId(cheepid).Result.AuthorLikeList.Add(userId);
        await _dbContext.SaveChangesAsync();
        return ReadCheepByCheepId(cheepid).Result.AuthorLikeList.Count;
    }
    public async Task<Cheep> ReadCheepByCheepId(int cheepid)
    {
        IQueryable<Cheep> query = Queryable.Where<Cheep>(_dbContext.Cheeps, cheep => cheep.CheepId == cheepid)
            .Select(cheep => cheep)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }

}
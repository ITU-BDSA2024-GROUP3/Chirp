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
        _nextCheepId = 0;//ensures the first cheep posted will have the ID of 0
    }
    /// <summary>
    /// Cheep gets created and added to the database
    /// CheepID is returned
    /// </summary>
    public async Task<int> CreateCheep(CheepDTO newMessage)
    {
        Author? cheepAuthor = ReadAuthorById(newMessage.UserId).Result;

        if (cheepAuthor == null)
        {
            throw new Exception("User not recognized!");
        }

        Cheep message = new Cheep(cheepId: _nextCheepId++, userId: newMessage.UserId, text: newMessage.Text, timeStamp: DateTime.Now, author: cheepAuthor, authorLikeList: new List<int>());
        var queryResult = _dbContext.Cheeps.Add(message); // does not write to the database!
        cheepAuthor.Cheeps.Add(message);


        await _dbContext.SaveChangesAsync(); // persist the changes in the database

        Console.WriteLine($"Store Cheep message = {message.Text} and UserId = {message.Author.UserId}");

        return queryResult.Entity.CheepId;
    }
    /// <summary>
    /// Returns a list of all authors that follow the author
    /// Also deletes any followers that no longer exists
    /// The cheeps returned is depended on the page the client is on 
    /// </summary>
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

        List<int> removeTheseIds = new List<int>();//authors that no longer exists

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
    /// <summary>
    /// Returns the cheeps displayed
    /// This is both the client's own cheeps and also the cheeps of the authors they follow
    /// </summary>
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
    /// <summary>
    /// Updates the text of the cheep
    /// When a cheep is updated, the timestamp is updated to when it got updated/edited
    /// </summary>
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
    /// <summary>
    /// Returns the cheeps displayed
    /// If a UserId is included, then it returns the cheeps for a private timeline
    /// This means only the cheeps posted by that specific author
    /// Otherwise, it'll return all the cheeps posted by everyone (public)
    /// The cheeps returned is depended on the page the client is on 
    /// </summary>
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
    /// <summary>
    /// Returns all cheeps with no regards to page
    /// If a UserId is included, then it returns the cheeps for a private timeline
    /// This means only the cheeps posted by that specific author
    /// Otherwise, it'll return all the cheeps posted by everyone (public)
    /// </summary>
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
    /// <summary>
    /// This only exists because another method uses it (following and unfollow)
    /// It actually belongs in AuthorRepo
    ///
    /// Returns the author with the id given 
    /// </summary>
    public async Task<Author?> ReadAuthorById(int id)
    {
        IQueryable<Author> query = Queryable.Where(_dbContext.Authors, author => author.UserId == id)
            .Select(author => author)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
    /// <summary>
    /// Converts unixTimeStamp to a date and time
    /// Automatically replaces full stops with a colon
    /// because some computers automatically formats dateTime to use full-stops instead of colons
    /// This was done because of tests
    /// </summary>
    public static string UnixTimeStampToDateTimeString(Int64 unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        // returns GMT Timezone 
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss").Replace('-', '/').Replace('.', ':');
    }
    /// <summary>
    /// The an author unfollows an author
    /// Returns the amount in the author's followingList
    /// </summary>
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
    /// <summary>
    /// Returns the amount of likes a cheep has
    /// Also removes the likes from authors that no longer exists
    /// </summary>
    public async Task<int> AmountOfLikes(int cheepId)
    {
        Cheep? cheep = await ReadCheepByCheepId(cheepId);
        
        if (cheep == null)
        {
            throw new Exception($"Cheep does not exist, AmountOfLikes in cheepRepo");
        }

        var removeThese = new List<int>();//authors that no longer exists are added here to be deleted later
        
        
        //if the list doens't exist it will be created here
        if (cheep.AuthorLikeList == null)
        {
            cheep.AuthorLikeList = new List<int>();
        }
        foreach (var authorid in cheep.AuthorLikeList)
        {
            Author? tempAuthor = await ReadAuthorById(authorid);
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
    /// <summary>
    /// Cheep gets unliked by the client who had previously liked it
    /// Removes the like
    /// Returns the amount of likes the cheep has
    /// </summary>
    public async Task<int> UnLikeCheep(int cheepid, int userId)
    {
        Cheep? cheep = await ReadCheepByCheepId(cheepid);
        Author? author = await ReadAuthorById(userId);
        
        if (cheep == null)
        {
            throw new Exception($"Cheep does not exist");
        }
        if (author == null)
        {
            throw new Exception($"Author does not exist");
        }
        if (!cheep.AuthorLikeList!.Contains(userId))
        {
            throw new Exception($"Did not like this cheep!");
        }
        
        cheep.AuthorLikeList.Remove(userId);

        await _dbContext.SaveChangesAsync();
        return cheep.AuthorLikeList.Count;
    }
    /// <summary>
    /// Author likes cheep
    /// Amount of likes is updated
    /// Returns the amount of likes the cheep has
    /// </summary>
    public async Task<int> LikeCheep(int cheepid, int userId)
    {
        Cheep? cheep = await ReadCheepByCheepId(cheepid);
        Author? author = await ReadAuthorById(userId);
        
        if (cheep == null)
        {
            throw new Exception($"Cheep does not exist");
        }
        if (author == null)
        {
            throw new Exception($"Author does not exist");
        }
        if (cheep.AuthorLikeList!.Contains(userId))
        {
            throw new Exception($"Already like this cheep!");
        }
        cheep.AuthorLikeList.Add(userId);
        await _dbContext.SaveChangesAsync();
        return cheep.AuthorLikeList.Count;
    }
    /// <summary>
    /// Returns the cheep that has the given id
    /// </summary>
    public async Task<Cheep?> ReadCheepByCheepId(int cheepid)
    {
        IQueryable<Cheep> query = Queryable.Where<Cheep>(_dbContext.Cheeps, cheep => cheep.CheepId == cheepid)
            .Select(cheep => cheep)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
    /// <summary>
    /// Looks through all cheeps to check if the given author has liked the cheep
    /// If they have, then the like gets removed
    /// The new likes gets saved in the database
    /// </summary>
    public async Task DeleteUserInformation(int userId)
    {
        foreach (Cheep cheep in _dbContext.Cheeps.Where(cheep => cheep.AuthorLikeList.Contains(userId)))
        {
            cheep.AuthorLikeList.Remove(userId);
        } 
        
        await _dbContext.SaveChangesAsync();
    }
}
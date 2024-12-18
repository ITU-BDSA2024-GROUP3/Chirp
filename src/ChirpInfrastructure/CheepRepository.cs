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
    /// Creates Cheep object and adds it to the database
    /// </summary>
    /// <param name="newMessage">CheepDTO object containing data for the new Cheep</param>
    /// <returns>CheepId of the created Cheep</returns>
    /// <exception cref="Exception">
    /// Exception - The Author with UserId equal to <c>newMessage.UserId</c> is not found in database
    /// </exception>
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
    /// Gets a list of AuthorDTO objects for all the users that the given user follows
    /// </summary>
    /// <param name="UserId">This corresponds to the UserId column in the database - not Id</param>
    /// <returns>A list of AuthorDTOs of all Authors that given user follows</returns>
    /// <exception cref="Exception">
    /// Exception - Author with UserId equal to <c>UserId</c> is not found -or-
    /// the Author's FollowingList is null - this should no longer occur
    /// </exception>
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
    /// Returns a list of CheepDTO containing cheeps from followed authors
    /// </summary>
    /// <param name="page">The page of private timeline being displayed</param>
    /// <param name="UserId">The Id of the user who is viewing their private timeline. This corresponds to the UserId column in the database - not Id</param>
    /// <returns>A list of CheepDTOs from followed authors</returns>
    /// <exception cref="Exception">
    /// Exception - Author with UserId equal to <c>UserId</c> is not found -or-
    /// the Author's FollowingList is null - this should no longer occur
    /// </exception>
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
    /// Updates the data of a cheep. The timestamp is updated to the time at which the Cheep was updated.
    /// </summary>
    /// <param name="updatedMessage">The new data of the cheep - the timestamp is ignored</param>
    /// <returns>The CheepId of the Cheep</returns>
    /// <exception cref="Exception">
    /// Exception - <c>updatedMessage</c> is null
    /// </exception>
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
    /// Gets a list of cheeps to be displayed. If a UserId is provided, returns the Cheeps for a private timeline. Otherwise, returns the Cheeps for a public timeline.
    /// </summary>
    /// <param name="page">The page being displayed</param>
    /// <param name="UserId">The UserId of the user</param>
    /// <returns></returns>
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
    /// If <c>UserId</c> is provided, returns all Cheeps made by that user. Otherwise, returns all Cheeps in the database.
    /// </summary>
    /// <param name="UserId">The UserId of the creator of the Cheeps. If none is provided, returns all Cheeps in the database.</param>
    /// <returns>All Cheeps made by given user, or simply all Cheeps in the database, depending on whether <c>UserId</c> is provided</returns>
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
    /// Gets Author object with given id.
    /// </summary>
    /// <param name="id">UserId of the Author to be returned</param>
    /// <returns>Author if one with given UserId is found - otherwise, <c>null</c></returns>
    public async Task<Author?> ReadAuthorById(int id)
    {
        IQueryable<Author> query = Queryable.Where(_dbContext.Authors, author => author.UserId == id)
            .Select(author => author)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
    
    /// <summary>
    /// Formats Unix tiemstamp to a string
    /// </summary>
    /// <param name="unixTimeStamp">Unix timestamp to be formatted</param>
    /// <returns>A string with date and time of given timestamp</returns>
    public static string UnixTimeStampToDateTimeString(Int64 unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        // returns GMT Timezone 
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp);
        return dateTime.ToString("MM/dd/yy H:mm:ss").Replace('-', '/').Replace('.', ':');
    }
    
    /// <summary>
    /// Set one Author to unfollow another
    /// </summary>
    /// <param name="wantToUnfollow">UserId of the Author that wants to unfollow another Author</param>
    /// <param name="wantToBeUnfollowed">UserId of the Author that the first Author wants to unfollow</param>
    /// <returns></returns>
    /// <exception cref="Exception">
    /// - Exception
    /// An Author tries to unfollow themselves -or-
    /// No Author with UserId equal to <c>wantToUnfollow</c> found in the database -or-
    /// The Author with UserId equal to <c>wantToUnfollow</c> is already following Author with UserId equal to <c>wantToBeUnfollowed</c>
    /// </exception>
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
    /// Gets the number of likes for a given cheep
    /// </summary>
    /// <param name="cheepId">CheepId of an existing cheep</param>
    /// <returns>The amount of likes the specified cheep has received</returns>
    /// <exception cref="Exception">
    /// Exception - No cheep with the given Id is found in the database
    /// </exception>
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
    /// Set given user to unlike a given cheep
    /// </summary>
    /// <param name="cheepid">Id of the cheep to be unliked</param>
    /// <param name="userId">UserId of the user</param>
    /// <returns>The amount of likes the Cheep has received after like has been removed</returns>
    /// <exception cref="Exception">
    /// Exception - Cheep with Id equal to <c>cheepid</c> not found -or-
    /// author with UserId equal to <c>userId</c> not found -or-
    /// author has not liked the specified cheep
    /// </exception>
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
    /// Set given user to like a given cheep
    /// </summary>
    /// <param name="cheepid">Id of the cheep to be liked</param>
    /// <param name="userId">UserId of the user</param>
    /// <returns>The amount of likes the Cheep has received after like has been added</returns>
    /// <exception cref="Exception">
    /// Exception - Cheep with Id equal to <c>cheepid</c> not found -or-
    /// author with UserId equal to <c>userId</c> not found -or-
    /// author has already liked the specified cheep
    /// </exception>
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
    /// Get Cheep with given Id
    /// </summary>
    /// <param name="cheepid">CheepId of the Cheep</param>
    /// <returns>Cheep with CheepId equal to <c>cheepid</c> if it exists - otherwise, <c>null</c></returns>
    public async Task<Cheep?> ReadCheepByCheepId(int cheepid)
    {
        IQueryable<Cheep> query = Queryable.Where<Cheep>(_dbContext.Cheeps, cheep => cheep.CheepId == cheepid)
            .Select(cheep => cheep)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
    
    /// <summary>
    /// Remmove all likes made by specified user
    /// </summary>
    /// <param name="userId">UserId of the user</param>
    public async Task DeleteUserInformation(int userId)
    {
        foreach (Cheep cheep in _dbContext.Cheeps.Where(cheep => cheep.AuthorLikeList.Contains(userId)))
        {
            cheep.AuthorLikeList.Remove(userId);
        } 
        
        await _dbContext.SaveChangesAsync();
    }
}
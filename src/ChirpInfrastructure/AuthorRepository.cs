using ChirpCore;
using ChirpCore.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace ChirpInfrastructure;

public class AuthorRepository: IAuthorRepository
{
    private readonly ChirpDBContext _dbContext;

    public AuthorRepository(ChirpDBContext context)
    {
        _dbContext = context;
    }
    
    /// <summary>
    /// Get the name of the user with given email address
    /// </summary>
    /// <param name="emailAddress">The email address of the user</param>
    /// <returns>The name of the user</returns>
    public async Task<string?> GetNameByEmail(string emailAddress)
    {
        Author? author = await ReadAuthorByEmail(emailAddress);

        if (author != null)
        {
            return author.Name;
        }

        return null;
    }
    
    /// <summary>
    /// Get AuthorDTO object containing data on user with the given ID
    /// </summary>
    /// <param name="id">This variable corresponds to the UserId column in the database - not the Id column</param>
    /// <returns>AuthorDTO containing data on the user</returns>
    public async Task<AuthorDTO?> ReadAuthorDTOById(int id)
    {
        IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.UserId == id)
            .Select(author => author.ToDTO())
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
    
    /// <summary>
    /// Get Author object containing data on user with the given ID
    /// </summary>
    /// <param name="id">This variable corresponds to the UserId column in the database - not the Id column</param>
    /// <returns>Author containing data on the user</returns>
    public async Task<Author?> ReadAuthorById(int id)
    {
        IQueryable<Author> query = Queryable.Where<Author>(_dbContext.Authors, author => author.UserId == id)
            .Select(author => author)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Get Author object containing data on user with the given name
    /// </summary>
    /// <param name="name">This variable corresponds to the Name column in the database - not the UserName column</param>
    /// <returns>Author containing data on the user</returns>
    public async Task<Author?> ReadAuthorByName(string name)
    {
        IQueryable<Author> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Name == name)
            .Select(author => author)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
    
    /// <summary>
    /// Get AuthorDTO object containing data on user with the given name
    /// </summary>
    /// <param name="email">This variable corresponds to the Email column in the database</param>
    /// <returns>AuthorDTO containing data on the user</returns>
    public async Task<AuthorDTO?> ReadAuthorDTOByEmail(string email)
    {
        IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Email == email)
            .Select(author => author.ToDTO())
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
    
    /// <summary>
    /// Get Author object containing data on user with the given name
    /// </summary>
    /// <param name="email">This variable corresponds to the Email column in the database</param>
    /// <returns>Author containing data on the user</returns>
    public async Task<Author?> ReadAuthorByEmail(string email)
    {
        IQueryable<Author> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Email == email)
            .Select(author => author)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Create Author for user with the given information
    /// </summary>
    /// <param name="name">Name of the user - corresponds to the Name column in the database</param>
    /// <param name="email">Email of the user - corresponds to the Email column in the database</param>
    /// <param name="UserId">User ID of the user - corresponds to the UserId column in the database</param>
    /// <returns>The created Author object</returns>
    public async Task<Author> CreateAuthor(string name, string email, int UserId)
    {
        Author author = new() { UserId = UserId, Name = name, Email = email, Cheeps = new List<Cheep>(), FollowingList = new List<int>()};
        var queryResult = await _dbContext.Authors.AddAsync(author); // does not write to the database!

        await _dbContext.SaveChangesAsync();

        return queryResult.Entity;
    }
    
    /// <summary>
    /// Gets the number of Authors in the database
    /// </summary>
    /// <returns>The number of Authors in the database</returns>
    public async Task<int> GetAuthorCount()
    {
        IQueryable<AuthorDTO> query =
            _dbContext.Authors.Select(author => author.ToDTO());
        return await query.CountAsync();
    }
    
    /// <summary>
    /// Set one Author to follow another
    /// </summary>
    /// <param name="wantToFollow">UserId of the Author that wants to follow another Author</param>
    /// <param name="wantToBeFollowed">UserId of the Author that the first Author wants to follow</param>
    /// <returns></returns>
    /// <exception cref="Exception">
    /// - Exception
    /// An Author tries to follow themselves -or-
    /// No Author with UserId equal to <c>wantToFollow</c> found in the database -or-
    /// The Author with UserId equal to <c>wantToFollow</c> is already following Author with UserId equal to <c>wantToBeFollowed</c>
    /// </exception>
    
    public async Task<int> Follow(int wantToFollow, int wantToBeFollowed)
    {
        if (wantToFollow == wantToBeFollowed)
        {
            throw new Exception($"Cannot follow yourself!");   
        } 
        
        var wantToFollowAuthor = ReadAuthorById(wantToFollow).Result;

        if (wantToFollowAuthor == null)
        {
            throw new Exception("User not recognized!");
        }
        
        if(wantToFollowAuthor.FollowingList.Contains(wantToBeFollowed))
        {
            throw new Exception($"Already following this author!");   
        }
        
        wantToFollowAuthor.FollowingList.Add(wantToBeFollowed);
        return await _dbContext.SaveChangesAsync();
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
        
        var wantToUnfollowAuthor = ReadAuthorById(wantToUnfollow).Result;
        
        if (wantToUnfollowAuthor == null)
        {
            throw new Exception("User not recognized!");
        }
        
        if(!wantToUnfollowAuthor.FollowingList.Contains(wantToBeUnfollowed))
        {
            throw new Exception($"Cannot unfollow - Not following this author");   
        }
        
        wantToUnfollowAuthor.FollowingList.Remove(wantToBeUnfollowed);
        return await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Get AuthorDTO object containing data on user with the given name.
    /// </summary>
    /// <param name="name">This variable corresponds to the Name column in the database - not the UserName column</param>
    /// <returns>AuthorDTO containing data on the user</returns>
    public async Task<AuthorDTO?> ReadAuthorDTOByName(string name)
    {
        IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Name == name)
            .Select(author => author.ToDTO())
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
}
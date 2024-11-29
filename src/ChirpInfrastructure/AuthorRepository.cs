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
    
    public async Task<string?> GetNameByEmail(string emailAddress)
    {
        Author? author = await ReadAuthorByEmail(emailAddress);

        if (author != null)
        {
            return author.Name;
        }

        return null;
    }
    
    public async Task<AuthorDTO?> ReadAuthorDTOById(int id)
    {
        IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.UserId == id)
            .Select(author => author.ToDTO())
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
    
    public async Task<Author?> ReadAuthorById(int id)
    {
        IQueryable<Author> query = Queryable.Where<Author>(_dbContext.Authors, author => author.UserId == id)
            .Select(author => author)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<Author?> ReadAuthorByName(string name)
    {
        IQueryable<Author> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Name == name)
            .Select(author => author)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
    public async Task<AuthorDTO?> ReadAuthorDTOByEmail(string email)
    {
        IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Email == email)
            .Select(author => author.ToDTO())
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
    
    public async Task<Author?> ReadAuthorByEmail(string email)
    {
        IQueryable<Author> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Email == email)
            .Select(author => author)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<Author> CreateAuthor(string name, string email, int UserId)
    {
        Author author = new() { UserId = UserId, Name = name, Email = email, Cheeps = new List<Cheep>(), FollowingList = new List<int>()};
        var queryResult = await _dbContext.Authors.AddAsync(author); // does not write to the database!

        await _dbContext.SaveChangesAsync();

        return queryResult.Entity;
    }
    
    public async Task<int> GetAuthorCount()
    {
        IQueryable<AuthorDTO> query =
            _dbContext.Authors.Select(author => author.ToDTO());
        return await query.CountAsync();
    }
    
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

    public async Task<AuthorDTO> ReadAuthorDTOByName(string name)
    {
        IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Name == name)
            .Select(author => author.ToDTO())
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }
}
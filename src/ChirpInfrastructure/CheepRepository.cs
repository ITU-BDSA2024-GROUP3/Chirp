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
        Author cheepAuthor = ReadAuthorById(newMessage.UserID).Result;

        Cheep message = new()
        {
            CheepId = _nextCheepId++, UserId = newMessage.UserID, Text = newMessage.Text, TimeStamp = DateTime.Now,
            Author = cheepAuthor
        };
        var queryResult = _dbContext.Cheeps.Add(message); // does not write to the database!
        cheepAuthor.Cheeps.Add(message);

        await _dbContext.SaveChangesAsync(); // persist the changes in the database

        Console.WriteLine($"Store Cheep message = {message.Text} and AuthorId = {message.Author.UserId}");

        return queryResult.Entity.CheepId;
    }

    public async Task<string> GetNameByEmail(string emailAddress)
    {
        //Check if email exists
        var emailExist = await ReadAuthorByEmail(emailAddress);
        if (emailExist == null)
        {
            Console.Error.WriteLine("Error: GetNameByEmail: Email address does not exist.");
        }
        else
        {
            //Check if email has name tied to it
            IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Email == emailAddress)
                .Select(author => new AuthorDTO() { Name = author.Name, UserId = author.UserId })
                .Take(1);
            var authordto = await query.FirstOrDefaultAsync();
            if (authordto == null)
            {
                Console.Error.WriteLine("Error: GetNameByEmail: Email address has no corresponding name");
            }
            else
            {
                return authordto.Name;
            }
        }
        //Don't want to be here
        return null;
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

    public async Task<AuthorDTO> ReadAuthorDTOById(int id)
    {
        IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.UserId == id)
            .Select(author => new AuthorDTO() { Name = author.Name, UserId = author.UserId })
            .Take(1);
        return await query.FirstOrDefaultAsync();
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
                    UserID = message.Author.UserId, 
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
                    UserID = message.Author.UserId,
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
                    UserID = message.Author.UserId,
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
                    UserID = message.Author.UserId,
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
        IQueryable<Author> query = Queryable.Where<Author>(_dbContext.Authors, author => author.UserId == id)
            .Select(author => author)
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<AuthorDTO> ReadAuthorByName(string name)
    {
        IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Name == name)
            .Select(author => new AuthorDTO() { Name = author.Name, UserId = author.UserId })
            .Take(1);
        return await query.FirstOrDefaultAsync();
    }

    public async Task<AuthorDTO> ReadAuthorByEmail(string email)
    {
        IQueryable<AuthorDTO> query = Queryable.Where<Author>(_dbContext.Authors, author => author.Email == email)
            .Select(author => new AuthorDTO() { Name = author.Name, UserId = author.UserId })
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
        IQueryable<AuthorDTO> query =
            _dbContext.Authors.Select(author => new AuthorDTO() { Name = author.Name, UserId = author.UserId });
        return await query.CountAsync();
    }
}
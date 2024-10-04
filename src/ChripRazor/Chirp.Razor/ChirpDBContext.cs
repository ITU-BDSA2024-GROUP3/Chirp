using Chirp.Razor.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Razor;

public class ChirpDBContext : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Cheep> Cheeps { get; set; }
    
    public ChirpDBContext(DbContextOptions<ChirpDBContext> options) : base(options)
    {
        
    }
}
using Microsoft.AspNetCore.Identity;

namespace ChirpCore.DomainModel;


public class Author : IdentityUser
{
    public int AuthorId { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; }
}
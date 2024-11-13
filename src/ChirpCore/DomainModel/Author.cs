using Microsoft.AspNetCore.Identity;

namespace ChirpCore.DomainModel;


public class Author : IdentityUser
{
    public int UserId { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; }
    
    public ICollection<Author> FollowingList { get; set; }
}
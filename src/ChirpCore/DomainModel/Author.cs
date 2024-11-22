using Microsoft.AspNetCore.Identity;

namespace ChirpCore.DomainModel;


public class Author : IdentityUser
{
    public int UserId { get; set; } 
    
    [PersonalData]
    public string Name { get; set; }
    
    [PersonalData]
    public string Email { get; set; }
    [PersonalData]
    public ICollection<Cheep> Cheeps { get; set; }
}
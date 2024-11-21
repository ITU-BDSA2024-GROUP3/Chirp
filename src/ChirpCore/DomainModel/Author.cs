using System.Collections;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;

namespace ChirpCore.DomainModel;


public class Author : IdentityUser
{
    public int UserId { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; }
    
    public IList<int> FollowingList { get; set; }
}
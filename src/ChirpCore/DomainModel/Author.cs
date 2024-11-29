using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace ChirpCore.DomainModel;


public class Author : IdentityUser
{
    [Key]
    public int UserId { get; set; } 
    
    [PersonalData]
    public string Name { get; set; }
    
    [PersonalData]
    public string Email { get; set; }
    [PersonalData]
    public ICollection<Cheep> Cheeps { get; set; }
    
    public IList<int> FollowingList { get; set; }
    
    public AuthorDTO ToDTO()
    {
        return new AuthorDTO
        {
            UserId = UserId, 
            Name = Name, 
            FollowingList = FollowingList
        };
    }
}
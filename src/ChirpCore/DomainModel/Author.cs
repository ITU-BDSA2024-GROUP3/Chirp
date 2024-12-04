using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace ChirpCore.DomainModel;


public class Author : IdentityUser
{
    #pragma warning disable 8618

    [Key]
    public int UserId { get; set; } 
    
    [PersonalData]
    public string Name { get; set; }
    
    [PersonalData]
    public new string Email { get; set; }
    [PersonalData]
    public ICollection<Cheep> Cheeps { get; set; }
    
    public IList<int> FollowingList { get; set; }
    
    public AuthorDTO ToDTO()
    {
        return new AuthorDTO(Name, UserId, FollowingList);
    }
    
    #pragma warning restore 8618
}
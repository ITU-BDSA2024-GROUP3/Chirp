using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
}
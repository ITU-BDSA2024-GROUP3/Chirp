using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace ChirpCore.DomainModel;

/// <summary>
/// This class represents the users of the program
/// This includes the premade users that don't have a login
/// </summary>
public class Author : IdentityUser
{
#pragma warning disable 8618

    [Key]
    public int UserId { get; set; } 
    
    [PersonalData]
    public string Name { get; set; }
    
    [PersonalData]
    public new string Email { get; set; }
    /// <summary>
    /// This is a list of all the cheeps this author has posted
    /// </summary>
    [PersonalData]
    public ICollection<Cheep> Cheeps { get; set; }
    /// <summary>
    /// This is a list of the IDs of authors who follow this author
    /// </summary>
    public IList<int> FollowingList { get; set; }
    /// <summary>
    /// This is a method that automatically creates a DTO of this specific author
    /// </summary>
    public AuthorDTO ToDTO()
    {
        return new AuthorDTO(Name, UserId, FollowingList);
    }
    
    #pragma warning restore 8618
}
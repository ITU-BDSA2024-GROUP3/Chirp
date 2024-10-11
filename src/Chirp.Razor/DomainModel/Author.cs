using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Razor.DomainModel;


public class Author
{
    public int AuthorId { get; set; } 
    public string Name { get; set; }
    public string Email { get; set; }
    public ICollection<Cheep> Cheeps { get; set; }
}
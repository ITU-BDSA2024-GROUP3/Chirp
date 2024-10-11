using System.ComponentModel.DataAnnotations;

namespace Chirp.Razor.ChirpInfrastucture;

public class Cheep
{
    public int CheepId { get; set; }
    [StringLength(160)]
    public string Text { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    public Author Author { get; set; }
    public int AuthorId { get; set; }
}
using System.ComponentModel.DataAnnotations;
using Chirp.Razor.ChirpInfrastucture;

namespace Chirp.Razor.ChirpCore;

public class CheepDTO
{
    [StringLength(160)]
    public string Text { get; set; }
    public Author Author { get; set; }
    public long TimeStamp { get; set; }

}
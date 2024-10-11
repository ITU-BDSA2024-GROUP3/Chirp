using System.ComponentModel.DataAnnotations;

namespace ChirpCore.DomainModel;

public class CheepDTO
{
    [StringLength(160)]
    public string Text { get; set; }
    public Author Author { get; set; }
    public long TimeStamp { get; set; }

}
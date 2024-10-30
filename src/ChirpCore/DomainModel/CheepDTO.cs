using System.ComponentModel.DataAnnotations;

namespace ChirpCore.DomainModel;

public class CheepDTO
{
    [StringLength(160)]
    public string Text { get; set; }
    public int AuthorID { get; set; }
    public string AuthorName { get; set; }
    public long TimeStamp { get; set; }

}
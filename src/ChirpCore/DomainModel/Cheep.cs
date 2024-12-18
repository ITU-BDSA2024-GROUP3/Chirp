using System.ComponentModel.DataAnnotations;

namespace ChirpCore.DomainModel;

/// <summary>
/// These represents the cheeps that are posted to the timelines for the appropriate users to read
/// These include the message, the amount of likes and the author who posted the cheep
/// </summary>
public class Cheep
{
    public int CheepId { get; set; }
    [StringLength(160)]
    public string Text { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    public Author Author { get; set; }
    public int UserId { get; set; }
    /// <summary>
    /// This is a list of the IDs of the authors who have liked the cheep
    /// </summary>
    public IList<int> AuthorLikeList { get; set; }
    /// <summary>
    ///constructor called by EFCore - Author is automatically set to non-null value after constructor finishes
    /// disable warning that Author is uninitialized    
    /// </summary>
    
    #pragma warning disable 8618
    
    public Cheep(int cheepId, string text, DateTimeOffset timeStamp, int userId, IList<int> authorLikeList)
    {
        CheepId = cheepId;
        Text = text;
        TimeStamp = timeStamp;
        UserId = userId; 
        AuthorLikeList = authorLikeList;
    }
    
    /// <summary>
    ///Constructor used by other classes
    /// </summary>
    
    #pragma warning restore 8618
    public Cheep(int cheepId, string text, DateTimeOffset timeStamp, Author author, int userId, IList<int> authorLikeList)
    {
        CheepId = cheepId;
        Text = text;
        TimeStamp = timeStamp;
        Author = author;
        UserId = userId;
        AuthorLikeList = authorLikeList;
    }
    /// <summary>
    /// This is a method that automatically creates a DTO of this specific cheep
    /// </summary>
    public CheepDTO ToDTO()
    {
        return new CheepDTO(text: Text!, userId: UserId, authorName: Author!.Name, timeStamp: TimeStamp.ToUnixTimeSeconds(), cheepId: CheepId, authorLikeList: AuthorLikeList);
    }
}
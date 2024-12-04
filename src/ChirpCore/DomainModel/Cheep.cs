using System.ComponentModel.DataAnnotations;

namespace ChirpCore.DomainModel;

public class Cheep
{
    public int CheepId { get; set; }
    [StringLength(160)]
    public string Text { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
    public Author Author { get; set; }
    public int UserId { get; set; }
    
    public IList<int> AuthorLikeList { get; set; }
    
    // constructor called by EFCore - Author is automatically set to non-null value after constructor finishes
    // disable warning that Author is uninitialized
    #pragma warning disable 8618
    public Cheep(int cheepId, string text, DateTimeOffset timeStamp, int userId, IList<int> authorLikeList)
    {
        CheepId = cheepId;
        Text = text;
        TimeStamp = timeStamp;
        UserId = userId; 
        AuthorLikeList = authorLikeList;
    }
    #pragma warning restore 8618
    
    // Constructor used by other classes
    public Cheep(int cheepId, string text, DateTimeOffset timeStamp, Author author, int userId, IList<int> authorLikeList)
    {
        CheepId = cheepId;
        Text = text;
        TimeStamp = timeStamp;
        Author = author;
        UserId = userId;
        AuthorLikeList = authorLikeList;
    }
    
    public CheepDTO ToDTO()
    {
        return new CheepDTO(text: Text!, userId: UserId, authorName: Author!.Name, timeStamp: TimeStamp.ToUnixTimeSeconds(), cheepId: CheepId, authorLikeList: AuthorLikeList);
    }
}
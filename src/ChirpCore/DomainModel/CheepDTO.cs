using System.ComponentModel.DataAnnotations;

namespace ChirpCore.DomainModel;

public class CheepDTO
{
    [StringLength(160)]
    public string Text { get; set; }
    public int UserId { get; set; }
    
    /// <summary>
    /// Name of the author - null when sent from client to server, not null when sent from server to client
    /// </summary>
    public string? AuthorName { get; set; }
    
    /// <summary>
    /// Timestamp of the cheep - null when sent from client to server, not null when sent from server to client.
    /// The timestamp is assigned by the server according to its clock
    /// </summary>
    public long? TimeStamp { get; set; }

    public CheepDTO(string text, int userId, string? authorName = null, long? timeStamp = null)
    {
        Text = text;
        UserId = userId;
        AuthorName = authorName;
        TimeStamp = timeStamp;
    }
}
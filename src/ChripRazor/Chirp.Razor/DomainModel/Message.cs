namespace Chirp.Razor.DomainModel;

public class Message
{
    public int MessageId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; }
    public User User { get; set; }
    public Int64 Timestamp { get; set; }
}
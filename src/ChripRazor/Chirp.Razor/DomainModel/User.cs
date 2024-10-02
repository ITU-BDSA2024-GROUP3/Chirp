namespace Chirp.Razor.DomainModel;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public ICollection<Message> Messages { get; set; }
}
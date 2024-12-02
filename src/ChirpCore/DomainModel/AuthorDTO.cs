namespace ChirpCore.DomainModel;

public class AuthorDTO
{
    public string Name { get; set; }
    public int UserId { get; set; }
    public IList<int> FollowingList { get; set; }
    
    
}
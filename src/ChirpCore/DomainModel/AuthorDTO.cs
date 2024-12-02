namespace ChirpCore.DomainModel;

public class AuthorDTO
{
    public string Name { get; set; }
    public int UserId { get; set; }
    public IList<int> FollowingList { get; set; }

    public AuthorDTO(string name, int userId, IList<int> followingList)
    {
        Name = name;
        UserId = userId;
        FollowingList = followingList;
    }
}
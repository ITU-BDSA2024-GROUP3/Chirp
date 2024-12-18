namespace ChirpCore.DomainModel;

/// <summary>
/// This includes the information about the author that is not needed for display
/// No sensitive information is included
/// </summary>
public class AuthorDTO
{
    public string Name { get; set; }
    public int UserId { get; set; }
    /// <summary>
    /// This is a list of the IDs of authors who follow this author
    /// </summary>
    public IList<int> FollowingList { get; set; }

    public AuthorDTO(string name, int userId, IList<int> followingList)
    {
        Name = name;
        UserId = userId;
        FollowingList = followingList;
    }
}
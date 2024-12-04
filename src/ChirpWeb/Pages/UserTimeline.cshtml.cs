
using System.ComponentModel.DataAnnotations;
using ChirpCore;
using ChirpCore.DomainModel;
using ChirpInfrastructure;
using ChirpWeb;
using Microsoft.AspNetCore.Authentication;
using ChirpWeb.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChirpWeb.Pages;

public class UserTimelineModel : CheepPostPage
{
    /// <summary>
    /// Author whose timeline is being viewed
    /// </summary>
    public AuthorDTO? Author { get; set; }
    public List<CheepDTO>? Cheeps { get; set; }
    public int currentPage;
    public UserTimelineModel(ICheepRepository CheepRepo, IAuthorRepository AuthorRepo) : base(CheepRepo, AuthorRepo) { }

    public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page)
    {
        TrySetLoggedInAuthor();
        Author = await _AuthorRepo.ReadAuthorDTOByName(name);

        if (Author == null)
        {
            throw new Exception("User not recognized");
        }
        
        //Own or other user's timeline
        if (User.Identity!.IsAuthenticated && LoggedInAuthor != null && Author.Name == name)
        {
            Cheeps = _CheepRepo.ReadFollowedCheeps(page, Author.UserId);
        }
        else
        {
            Cheeps= _CheepRepo.ReadCheeps(page, Author.UserId);
        }
        
        currentPage = page;

        if (currentPage < 1)
        {
            currentPage = 1;
        }

        return Page();
    }
}


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
    public Author? author { get; set; }
    public AuthorDTO Author { get; set; }
    public List<CheepDTO> Cheeps { get; set; }
    public int currentPage;
    public UserTimelineModel(ICheepRepository CheepRepo, IAuthorRepository AuthorRepo) : base(CheepRepo, AuthorRepo) { }

    public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page)
    {
        author = await _AuthorRepo.ReadAuthorByEmail(User.Identity.Name);
        setUsername();
        var authorTask = await _AuthorRepo.ReadAuthorByName(name);
        

        //await Task.WhenAll(authorTask, cheepsTask);


        //private or public timeline
        if (User.Identity.IsAuthenticated && author.Name == name)
        {
            Cheeps = _CheepRepo.ReadFollowedCheeps(page, authorTask.UserId);
        }
        else
        {
            Cheeps= _CheepRepo.ReadCheeps(page, authorTask.UserId);
        }
        
        Author = new AuthorDTO() { Name = authorTask.Name, UserId = authorTask.UserId};
        
        currentPage = page;

        if (currentPage < 1)
        {
            currentPage = 1;
        }

        return Page();
    }
}

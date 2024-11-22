
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
    public UserTimelineModel(ICheepRepository repo) : base(repo) { }

    public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page)
    {
        author = await _repo.ReadAuthorByEmail(User.Identity.Name);
        setUsername();
        var authorTask = await _repo.ReadAuthorByName(name);
        

        //await Task.WhenAll(authorTask, cheepsTask);

        Author = authorTask;

        //private or public timeline
        if (User.Identity.IsAuthenticated && author.Name == name)
        {
            Cheeps = await _repo.ReadFollowedCheeps(page, authorTask.UserId);
        }
        else
        {
            Cheeps= await _repo.ReadCheeps(page, authorTask.UserId);
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

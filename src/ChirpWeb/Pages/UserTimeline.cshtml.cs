
using System.ComponentModel.DataAnnotations;
using ChirpCore.DomainModel;
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
    public UserTimelineModel(ICheepService service) : base(service) { }

    public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page)
    {
        author = await _service.ReadAuthorByEmail(User.Identity.Name);
        
        // We think, that if FollowingList is empty, it will be read as null from the database
        if (author.FollowingList == null)
        {
            author.FollowingList = new List<Author>();
        }
        
        setUsername();
        var authorTask = await _service.ReadAuthorByName(name);

        var cheepsTask = await _service.GetCheepsFromAuthor(authorTask.UserId, page);

        //await Task.WhenAll(authorTask, cheepsTask);

        Author = authorTask;
        Cheeps = cheepsTask;

        currentPage = page;

        if (currentPage < 1)
        {
            currentPage = 1;
        }

        return Page();
    }
}

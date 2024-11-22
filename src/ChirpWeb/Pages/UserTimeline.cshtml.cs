
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
    public AuthorDTO Author { get; set; }
    public List<CheepDTO> Cheeps { get; set; }
    public int currentPage;
    public UserTimelineModel(ICheepRepository repo) : base(repo) { }

    public async Task<ActionResult> OnGetAsync(string name, [FromQuery] int page)
    {
        setUsername();
        var authorTask = await _repo.ReadAuthorByName(name);
        
        var cheepsTask = await _repo.ReadCheeps(page, authorTask.UserId);

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

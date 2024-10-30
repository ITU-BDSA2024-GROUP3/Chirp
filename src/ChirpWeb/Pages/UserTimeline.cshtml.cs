
using System.ComponentModel.DataAnnotations;
using ChirpCore.DomainModel;
using ChirpWeb;
using ChirpWeb.Pages.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChirpWeb.Pages;

public class UserTimelineModel : CheepPostPage
{
    public AuthorDTO Author { get; set; }
    public List<CheepDTO> Cheeps { get; set; }
    public int currentPage;
    
    public UserTimelineModel(ICheepService service) : base(service) { }

    public async Task<ActionResult> OnGetAsync(string author, [FromQuery] int page)
    {
        var authorTask = await _service.ReadAuthorByName(author);
        
        Console.WriteLine("page: "+page);
        Console.WriteLine("UserId: "+authorTask);
        Console.WriteLine("UserId: "+authorTask.UserId);

        
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


using ChirpCore.DomainModel;
using ChirpWeb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChirpWeb.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public AuthorDTO Author { get; set; }
    public List<CheepDTO> Cheeps { get; set; }
    public int currentPage;

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public async Task<ActionResult> OnGetAsync(string author, [FromQuery] int page)
    {
        var authorTask = await _service.ReadAuthorByName(author);
        
        Console.WriteLine("page: "+page);
        Console.WriteLine("UserId: "+author);
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

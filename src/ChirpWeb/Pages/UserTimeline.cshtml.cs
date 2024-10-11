
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

    public async Task<ActionResult> OnGetAsync(int userId, [FromQuery] int page)
    {
        var authorTask = _service.GetAuthor(userId);
        var cheepsTask = _service.GetCheepsFromAuthor(userId, page);

        await Task.WhenAll(authorTask, cheepsTask);
        
        Author = authorTask.Result;
        Cheeps = cheepsTask.Result;
        
        currentPage = page;

        if (currentPage < 1)
        {
            currentPage = 1;
        }

        return Page();
    }
}

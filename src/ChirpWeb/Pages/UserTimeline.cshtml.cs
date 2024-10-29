
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

    public async Task<ActionResult> OnGetAsync(string userEmail, [FromQuery] int page)
    {
        var authorTask = _service.ReadAuthorByEmail(userEmail);
        AuthorDTO author = authorTask.Result;

        if (author == null)
        {
            return NotFound();
        }
        
        var cheepsTask = _service.GetCheepsFromAuthor(author.UserId, page);

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

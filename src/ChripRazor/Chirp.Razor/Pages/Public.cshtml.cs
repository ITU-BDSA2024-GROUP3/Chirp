using Chirp.Razor.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }
    public int currentPage;
    
    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public async Task<ActionResult> OnGetAsync([FromQuery] int page)
    {
        currentPage = page;
        Cheeps = await _service.GetCheeps(currentPage);

        if (currentPage < 1)
        {
            currentPage = 1;
        }

        return Page();
    }
}

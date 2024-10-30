using ChirpCore.DomainModel;
using ChirpWeb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ChirpWeb.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; }
    public int currentPage;
    
    [BindProperty]
    [Required]
    [MaxLength(160)]
    public string Text { get; set; }
    
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

    public async Task<ActionResult> OnPost()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage("Public");
        }
        
        if (!ModelState.IsValid)
        {
            //do something
            return RedirectToPage("Public");
        }

        // Create cheep
        //Author author = (User.Identity.Name)
        CheepDTO newCheep = new CheepDTO() { Text = Text, Author = _service.GetAuthor(1).Result };
        _service.CreateCheep(newCheep);
        
        return RedirectToPage("Public");
    }
}

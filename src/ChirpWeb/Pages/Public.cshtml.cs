using ChirpCore.DomainModel;
using ChirpWeb;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ChirpWeb.Pages.Shared;

namespace ChirpWeb.Pages;

public class PublicModel : CheepPostPage
{
    public List<CheepDTO> Cheeps { get; set; }
    public int currentPage;
    
    public PublicModel(ICheepService service) : base(service) { }

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

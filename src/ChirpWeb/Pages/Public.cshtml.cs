using ChirpCore.DomainModel;
using ChirpWeb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using ChirpWeb.Pages.Shared;

namespace ChirpWeb.Pages;

public class PublicModel : CheepPostPage
{
    public List<CheepDTO> Cheeps { get; set; }
    public int currentPage;
    
    public Author author { get; set; }
    
    public PublicModel(ICheepService service) : base(service) { }

    public async Task<ActionResult> OnGetAsync([FromQuery] int page)
    {
        if (User.Identity.IsAuthenticated)
        {
            author = await _service.ReadAuthorByEmail(User.Identity.Name);
            
            // We think, that if FollowingList is empty, it will be read as null from the database
            
        }        
        
        setUsername();
        
        currentPage = page;
        Cheeps = await _service.GetCheeps(currentPage);
        if (currentPage < 1)
        {
            currentPage = 1;
        }

        return Page();
    }
}

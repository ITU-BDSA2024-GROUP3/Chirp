using System.ComponentModel.DataAnnotations;
using ChirpCore.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChirpWeb.Pages.Shared;

public class CheepPostPage : PageModel
{
    protected readonly ICheepService _service;
    
    [BindProperty]
    [Required]
    [MaxLength(160)]
    public string Text { get; set; }
    
    public CheepPostPage(ICheepService service)
    {
        _service = service;   
    }
    
    
    public async Task<ActionResult> OnPost()
    {
        /*
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToPage("Public");
        }
        */
        
        if (!ModelState.IsValid)
        {
            //do something
            return RedirectToPage("Public");
        }

        CheepDTO newCheep = new CheepDTO() { Text = Text, AuthorID = 1};
        _service.CreateCheep(newCheep);
        
        return RedirectToPage("Public");
    }
}
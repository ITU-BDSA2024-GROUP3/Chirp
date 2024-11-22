using ChirpCore.DomainModel;
using ChirpWeb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ChirpCore;
using ChirpWeb.Pages.Shared;

namespace ChirpWeb.Pages;

public class PublicModel : CheepPostPage
{
    public List<CheepDTO> Cheeps { get; set; }
    public int currentPage;
    
    public PublicModel(ICheepRepository repo) : base(repo) { }

    public async Task<ActionResult> OnGetAsync([FromQuery] int page)
    {
        setUsername();
        currentPage = page;
        Cheeps = await _repo.ReadCheeps(currentPage, null);
        if (currentPage < 1)
        {
            currentPage = 1;
        }

        return Page();
    }
    
    
}

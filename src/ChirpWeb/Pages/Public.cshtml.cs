using ChirpCore.DomainModel;
using ChirpWeb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using ChirpCore;
using ChirpWeb.Pages.Shared;

namespace ChirpWeb.Pages;

public class PublicModel : CheepPostPage
{
    public List<CheepDTO> Cheeps { get; set; }
    public int currentPage;
    
    public PublicModel(ICheepRepository CheepRepo, IAuthorRepository AuthorRepo) : base(CheepRepo, AuthorRepo) { }
    
    public Author author { get; set; }
    

    public async Task<ActionResult> OnGetAsync([FromQuery] int page)
    {
        if (User.Identity.IsAuthenticated)
        {
            author = await _AuthorRepo.ReadAuthorByEmail(User.Identity.Name);
            
            // We think, that if FollowingList is empty, it will be read as null from the database
            
        }        
        
        setUsername();
        
        currentPage = page;
        Cheeps = _CheepRepo.ReadCheeps(currentPage, null);
        if (currentPage < 1)
        {
            currentPage = 1;
        }

        return Page();
    }
}

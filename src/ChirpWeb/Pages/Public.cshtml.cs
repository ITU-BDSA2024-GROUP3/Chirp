using ChirpCore.DomainModel;
using ChirpWeb;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using ChirpCore;
using ChirpWeb.Pages.Shared;
using SQLitePCL;

namespace ChirpWeb.Pages;

public class PublicModel : CheepPostPage
{
    public List<CheepDTO> Cheeps { get; set; }

    public int currentPage { get; set; }//current page that the user is on
    
    #pragma warning disable 8618
    public PublicModel(ICheepRepository CheepRepo, IAuthorRepository AuthorRepo) : base(CheepRepo, AuthorRepo)
    { }
    #pragma warning restore 8618

    /// <summary>
    ///Reads all the cheeps and ensures pagenation 
    /// </summary>
    public ActionResult OnGet([FromQuery] int page)
    {
        TrySetLoggedInAuthor();
        
        currentPage = page;
        Cheeps = _CheepRepo.ReadCheeps(currentPage, null);
        if (currentPage < 1)
        {
            currentPage = 1;
        }

        return Page();
    }
    
}

using System.ComponentModel.DataAnnotations;
using ChirpCore;
using ChirpCore.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChirpWeb.Pages.Shared;

public class BasePage : PageModel
{
    protected readonly ICheepRepository _CheepRepo;
    protected readonly IAuthorRepository _AuthorRepo;
    
    public AuthorDTO? LoggedInAuthor { get; set; }
    
    public BasePage(ICheepRepository CheepRepo, IAuthorRepository AuthorRepo)
    {
        _CheepRepo = CheepRepo;
        _AuthorRepo = AuthorRepo;
    } 
    
    public async void TrySetLoggedInAuthor() {
        if (User.Identity!.IsAuthenticated && LoggedInAuthor == null)
        {
            LoggedInAuthor = await _AuthorRepo.ReadAuthorDTOByEmail(User.Identity.Name!);
        }
    }





}
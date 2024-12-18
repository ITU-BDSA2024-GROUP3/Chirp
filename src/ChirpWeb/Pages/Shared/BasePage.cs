using System.ComponentModel.DataAnnotations;
using ChirpCore;
using ChirpCore.DomainModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChirpWeb.Pages.Shared;

/// <summary>
/// This class in inherited by Page Models that need access to the Author object of the user that is logged in
/// </summary>

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
    
    /// <summary>
    /// Sets <c>LoggedInAuthor</c> to the Author object of the user, if they have been authenticated
    /// </summary>
    public async void TrySetLoggedInAuthor() {
        if (User.Identity!.IsAuthenticated && LoggedInAuthor == null)
        {
            LoggedInAuthor = await _AuthorRepo.ReadAuthorDTOByEmail(User.Identity.Name!);//User.Identity.Name is the email
        }
    }
}